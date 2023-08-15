using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using Noggog;
using Mutagen.Bethesda.Plugins.Records;
using Noggog.StructuredStrings;
using DynamicData;
using System.Collections.Immutable;
using CommandLine;
using Mutagen.Bethesda.Plugins.Order;

namespace SRExteriorCitiesPatcher
{
    public class Program
    {
        // Cells and Worldspaces
        internal static Dictionary<Tuple<P2Int, FormKey>, IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>> originalCellGrid = new();
        internal static Dictionary<P2Int, IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>> tamrielCellGrids = new();
        internal static HashSet<P2Int> originalCells = new();

        internal static IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>? tamrielPersistentCellContext;
        internal static IFormLinkGetter<ICellGetter> tamrielPersistentCell = new FormLink<ICellGetter>(FormKey.Factory("000D74:Skyrim.esm"));

        // SR Exterior Cities main plugin
        internal static ModKey srexMain;
        internal static bool srexModExists = false;
        internal static HashSet<FormKey> SREXMainFormIDs = new();

        // Doors
        internal static HashSet<FormKey> allDoors = new();

        // Counters
        internal static int nbTotal = 0;
        internal static int nbTempTotal = 0;
        internal static int nbPersistTotal = 0;

        // Worldspaces to move to Tamriel
        internal static List<FormKey> worldspacesToMove = new()
        {
            Skyrim.Worldspace.WhiterunWorld.FormKey,
            Skyrim.Worldspace.SolitudeWorld.FormKey,
            Skyrim.Worldspace.WindhelmWorld.FormKey,
            Skyrim.Worldspace.RiftenWorld.FormKey,
            Skyrim.Worldspace.MarkarthWorld.FormKey
        };

        private static readonly HashSet<ModKey> vanillaModKeys = new()
        {
            ModKey.FromNameAndExtension("Skyrim.esm"),
            ModKey.FromNameAndExtension("Update.esm"),
            ModKey.FromNameAndExtension("Dawnguard.esm"),
            ModKey.FromNameAndExtension("HearthFires.esm"),
            ModKey.FromNameAndExtension("Dragonborn.esm")
        };

        private static readonly List<FormKey> doorsToExclude = new()
        {
            FormKey.Factory("042283:Skyrim.esm"), // Rften main gate
            FormKey.Factory("042285:Skyrim.esm"), // Rften main gate min use
            FormKey.Factory("055FCA:Skyrim.esm"), // Windhelm main gate
            FormKey.Factory("0A17D3:Skyrim.esm"), // Windhelm main gate 2
            FormKey.Factory("101D9D:Skyrim.esm"), // Solitude main gate
            FormKey.Factory("037F1B:Skyrim.esm"), // Solitude main gate 2
            FormKey.Factory("01C38B:Skyrim.esm"), // Markarth main gate
        };

        /**
         * Moves placed objects/NPCs/hazards from the city worldspace to the corresponding Tamriel cell
         */
        internal static void DoSimpleMove(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, IModContext<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter> placed)
        {
            // Get parent cell 
            placed.TryGetParentSimpleContext<ICellGetter>(out var cell);

            // Ignore null 
            if (cell is null || cell.Record is null || cell.Record.Grid is null) return;
            if (placed is null) return;

            // Get the parent worldspace
            placed.TryGetParentSimpleContext<IWorldspaceGetter>(out var parent);
            if (parent is null || parent.Record is null) return;

            // Object in worldspaces
            if (worldspacesToMove.Contains(parent.Record.FormKey))
            {
                if (Settings.debug)
                    System.Console.WriteLine("Object found in worldspace!");

                
                // Get the relevant cells
                if (!tamrielCellGrids.TryGetValue(cell.Record.Grid.Point, out var tamrielCellContext)) return;
                if (!originalCellGrid.TryGetValue(new Tuple<P2Int, FormKey>(cell.Record.Grid.Point, cell.Record.FormKey), out var originalCellContext)) return;

                // Get the original 
                var original = originalCellContext.GetOrAddAsOverride(state.PatchMod);
                if (original is null) return;

                // Open/copy the PlacedObject in the patch mod
                var placedState = placed.GetOrAddAsOverride(state.PatchMod);


                // Move persistent objects to the Tamriel Persistent cell
                if (original.Persistent.Contains(placedState))
                {
                    if (tamrielPersistentCellContext is null) return;

                    // Get the Tamriel Persistent cell
                    var tamPersistCell = tamrielPersistentCellContext.GetOrAddAsOverride(state.PatchMod);
                    if (tamPersistCell is null) return;

                    // Remove from the original worldspace cell and move to the Tamriel Persistent cell
                    original.Persistent.Remove(placedState);
                    tamPersistCell.Persistent.Add(placedState);

                    // Count
                    nbPersistTotal++;
                    nbTotal++;

                    if (Settings.debug)
                        System.Console.WriteLine("Persistent object moved from " + parent.Record.EditorID + " " + original.Grid?.Point.ToString() + " to " + tamPersistCell.FormKey);
                }

                // Move the temporary object to the Tamriel worldspace
                if (original.Temporary.Contains(placedState))
                {
                    var tamriel = tamrielCellContext.GetOrAddAsOverride(state.PatchMod);
                    if (tamriel is null) return;

                    // Remove from the original worldspace cell and move to the corresponding Tamriel cell
                    original.Temporary.Remove(placedState);
                    tamriel.Temporary.Add(placedState);
                    //tamriel.Location.SetTo(original.Location);

                    // Count
                    nbTempTotal++;
                    nbTotal++;

                    if (Settings.debug)
                        System.Console.WriteLine("Temporary object moved from " + parent.Record.EditorID + " " + original.Grid?.Point.ToString() + " to " + tamriel.Grid?.Point.ToString());
                }
            }

            // Show progress every 1000 records moved
            if (nbTotal % 1000 == 0 && nbTotal > 0)
                System.Console.WriteLine("Moved " + nbTotal + " objects...");
        }
    

        /**
         * Fill in dictionaries of the valid cells, both in the city Worldspaces and the corresponding Tamriel cells
         */
        internal static void DoWorldspaceMapping(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach (var cellContext in state.LoadOrder.ListedOrder.Cell().WinningContextOverrides(state.LinkCache))
            {
                // Ignore null
                if (cellContext is null || cellContext.Record is null) continue;

                var cell = cellContext.Record;

                // Filter out interior cells
                if (cell.Flags.HasFlag(Cell.Flag.IsInteriorCell)) continue;

                // Filter out cells with no Grid
                if (cell.Grid is null) continue;

                // Try getting the worldspace
                if (!cellContext.TryGetParent<IWorldspaceGetter>(out var worldspace)) continue;

                // Tamriel worldspace
                if (worldspace.FormKey.Equals(Skyrim.Worldspace.Tamriel.FormKey))
                {
                    // Tamriel Persistent Cell
                    if (tamrielPersistentCellContext is null
                        && cellContext.Record.FormKey.Equals(tamrielPersistentCell.FormKey))
                    {
                        tamrielPersistentCellContext = cellContext;

                        System.Console.WriteLine("Found Tamriel persistent cell! " + tamrielPersistentCellContext.Record.FormKey);
                    }
                    else
                    {
                        bool test = tamrielCellGrids.TryAdd(cell.Grid.Point, cellContext);

                        if (Settings.debug && !test)
                            System.Console.WriteLine("Duplicate cell found at " + cell.Grid.Point.X + "," + cell.Grid.Point.Y);
                    }
                }
                // Another of the accepted worldspaces
                else if (worldspacesToMove.Contains(worldspace.FormKey))
                {
                    originalCellGrid.TryAdd(new Tuple<P2Int, FormKey>(cell.Grid.Point, cell.FormKey), cellContext);
                    
                    if(!originalCells.Contains(cell.Grid.Point))
                        originalCells.Add(cell.Grid.Point);
                }
            }
        }



        /* =================================================== \\
        || --------------------------------------------------- ||
        ||                                                     ||
        ||                     RUN PATCHER                     ||
        ||                                                     ||
        || --------------------------------------------------- ||
        \\ =================================================== */

        public static Lazy<Settings> _settings = null!;
        public static Settings Settings => _settings.Value;

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings("Settings", "settings.json", out _settings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynthesisSREX2.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            /// Variables and initialisation
            // Create a link cache
            ILinkCache cache = state.LinkCache;


            // Try to get the main mod
            ModKey.TryFromFileName("SR Exterior Cities.esp", out var srexMainTmp);
            srexMain = srexMainTmp;

            if (state.LoadOrder.ListedOrder.ModExists(srexMain))
            {
                //if(Settings.debug)
                    System.Console.WriteLine("Main SE Exterior plugin detected!");

                srexModExists = true;

                // Add it as master
                MasterReference m = new() { Master = srexMain, FileSize = 0 };
                state.PatchMod.ModHeader.MasterReferences.Add(m);
            }
            else //if (Settings.debug)
                System.Console.WriteLine("WARNING: Main SE Exterior plugin not detected!");
            

            /// Mapping out all relevant cells in Tamriel and other worldspaces 
            System.Console.WriteLine("Mapping out the worldspaces...");
            DoWorldspaceMapping(state);
            System.Console.WriteLine("All worldspaces mapped!");


            /* --------------------------------------------------- \
            |             FORM LOADORDER TO CONSIDER               |
            \ --------------------------------------------------- */

            IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>> loadOrderToConsiderForObjects;
            IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>> loadOrderToConsiderForNPCs;
            IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, IAPlacedTrap, IAPlacedTrapGetter>> loadOrderToConsiderForHazards;
            if (srexModExists)
            {
                var modsWithoutSREXMain = state.LoadOrder.PriorityOrder.Where(x => !x.ModKey.Equals(srexMain) && !vanillaModKeys.Contains(x.ModKey));

                loadOrderToConsiderForObjects = modsWithoutSREXMain.PlacedObject().WinningContextOverrides(state.LinkCache);
                loadOrderToConsiderForNPCs = modsWithoutSREXMain.PlacedNpc().WinningContextOverrides(state.LinkCache);
                loadOrderToConsiderForHazards = modsWithoutSREXMain.APlacedTrap().WinningContextOverrides(state.LinkCache);
            }
            else
            {
                loadOrderToConsiderForObjects = state.LoadOrder.PriorityOrder.PlacedObject().WinningContextOverrides(state.LinkCache);
                loadOrderToConsiderForNPCs = state.LoadOrder.PriorityOrder.PlacedNpc().WinningContextOverrides(state.LinkCache);
                loadOrderToConsiderForHazards = state.LoadOrder.PriorityOrder.APlacedTrap().WinningContextOverrides(state.LinkCache);
            }


            /* --------------------------------------------------- \
            |                    MOVE OBJECTS                      |
            \ --------------------------------------------------- */
            
            System.Console.WriteLine("Moving objects...");
            foreach (var placed in loadOrderToConsiderForObjects)
            {
                // Ignore occlusion planes
                if (Settings.ignoreOcclusion && placed.Record.Base.FormKey.Equals(Skyrim.Static.PlaneMarker.FormKey))
                {
                    if (Settings.debug)
                        System.Console.WriteLine("Occlusion plane ignored in worldspace!");
                    continue;
                }

                // When the main mod exists, the doors need specific handling
                placed.Record.Base.TryResolve<IDoorGetter>(state.LinkCache, out var door);
                if (door is not null) continue;

                DoSimpleMove(state, placed);
            }
            System.Console.WriteLine("Moved " + nbTotal + " objects (" + nbPersistTotal + " persistent + " + nbTempTotal + " temporary objects)");

            // Reset counters
            nbTotal = 0;
            nbTempTotal = 0;
            nbPersistTotal = 0;

            // NPCs
            foreach (var placed in loadOrderToConsiderForNPCs)
            {
                DoSimpleMove(state, placed);
            }
            System.Console.WriteLine("Moved " + nbTotal + " NPCs (" + nbPersistTotal + " persistent + " + nbTempTotal + " temporary NPCs)");


            // Reset counters
            nbTotal = 0;
            nbTempTotal = 0;
            nbPersistTotal = 0;

            // Hazards
            foreach (var placed in loadOrderToConsiderForHazards)
            {
                DoSimpleMove(state, placed);
            }
            System.Console.WriteLine("Moved " + nbTotal + " hazards (" + nbPersistTotal + " persistent + " + nbTempTotal + " temporary hazards)");
            System.Console.WriteLine("Done retrieving placed objects from main plugin!");

            /* --------------------------------------------------- \
            |                     HANDLE DOORS                     |
            \ --------------------------------------------------- */

            // Counter
            int nbDoors = 0;

            // All doors modified by mods modified doors
            Dictionary<FormKey, IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>> srexDoors = new();

            // Only if SR Exterior Cities main plugin is active
            if (srexModExists)
            {
                // Get only SREX modified doors
                var modsSREXorPatches = state.LoadOrder.PriorityOrder.Where(x => x.ModKey.Equals(srexMain)
                                                                                 || x.ModKey.FileName.String.Contains("SREX_", StringComparison.Ordinal));

                System.Console.WriteLine("Mapping out SR Exterior Cities (and patches) modified doors");
                foreach (var obj in modsSREXorPatches.PlacedObject().WinningContextOverrides(cache))
                {
                    // Ignore Null
                    if (obj is null) continue;

                    // Ignore if not a door
                    obj.Record.Base.TryResolve<IDoorGetter>(state.LinkCache, out var door);
                    if (door is null) continue;

                    srexDoors.TryAdd(obj.Record.FormKey, obj);
                }
                System.Console.WriteLine("Doors modified SREX or its patches): " + srexDoors.Count);
            }

            // Get only Mod modified doors (exclude vanilla and SREX)
            var modsWithoutVanillaOrSREX = state.LoadOrder.PriorityOrder.Where(x => !vanillaModKeys.Contains(x.ModKey)
                                                                                && !x.ModKey.Equals(srexMain)
                                                                                && !x.ModKey.FileName.String.Contains("SREX_", StringComparison.Ordinal));


            System.Console.WriteLine("Moving all doors modified by mods...");
            foreach (var obj in modsWithoutVanillaOrSREX.PlacedObject().WinningContextOverrides(cache))
            {
                // Ignore Null
                if (obj is null) continue;

                // Ignore if not a door
                obj.Record.Base.TryResolve<IDoorGetter>(state.LinkCache, out var door);
                if (door is null) continue;


                // Get the parent worldspace
                obj.TryGetParentSimpleContext<IWorldspaceGetter>(out var parent);
                if (parent is null || parent.Record is null) continue;

                // Ignore if it is a blacklisted door (city gates)
                if (doorsToExclude.Contains(obj.Record.FormKey)) continue;

                // If the door was edited by SREX too
                if (srexDoors.ContainsKey(obj.Record.FormKey))
                {
                    // Only update the placement
                    srexDoors.TryGetValue(obj.Record.FormKey, out var value);
                    if (value is null) continue;

                    Placement placement = new();
                    if (obj.Record.Placement is not null)
                        placement = new()
                        {
                            Position = obj.Record.Placement.Position,
                            Rotation = obj.Record.Placement.Rotation
                        };


                    EnableParent enableParent = new();
                    if (obj.Record.EnableParent is not null)
                        enableParent = new()
                        {
                            Flags = obj.Record.EnableParent.Flags,
                            Reference = obj.Record.EnableParent.Reference.AsSetter(),
                            Versioning = obj.Record.EnableParent.Versioning
                        };

                    int majorFlags = obj.Record.MajorRecordFlagsRaw;
                    SkyrimMajorRecord.SkyrimMajorRecordFlag flags = obj.Record.SkyrimMajorRecordFlags;
                    IFormLink<ILocationRecordGetter> location = obj.Record.LocationReference.AsSetter();


                    var placedState = value.GetOrAddAsOverride(state.PatchMod);
                    if (placedState is null) continue;

                    // Only alter the placement, flags, Enable Parent, Flags and Location
                    if (obj.Record.Placement is not null)
                        placedState.Placement = placement;

                    if (obj.Record.EnableParent is not null)
                        placedState.EnableParent = enableParent;

                    placedState.MajorRecordFlagsRaw = majorFlags;

                    placedState.SkyrimMajorRecordFlags = flags;

                    if (!location.IsNull)
                        placedState.LocationReference = location.AsNullable();

                    if(Settings.debug)
                    {
                        System.Console.WriteLine("Door found, already modified by SREX");
                    }

                    // Counter
                    nbDoors++;

                    continue;
                }
                else
                {
                    // Ignore if it is in Tamriel
                    if (parent.Record.FormKey.Equals(Skyrim.Worldspace.Tamriel.FormKey)) continue;

                    // Move the door
                    DoSimpleMove(state, obj);

                    // Counter
                    nbDoors++;
                }
            }
            System.Console.WriteLine("Doors moved: " + nbDoors);
            System.Console.WriteLine("Done handling Doors!");

            /* =================================================== \\
            || --------------------------------------------------- ||
            ||                                                     ||
            ||             NAVMESH (ONLY FOR PATCHMAKERS)          ||
            ||                                                     ||
            || --------------------------------------------------- ||
            \\ =================================================== */

            /// Navmeshes
            if (Settings.enableNavmeshEdit)
            {
                foreach (var navmesh in state.LoadOrder.PriorityOrder.NavigationMesh().WinningContextOverrides(state.LinkCache))
                {
                    // Get parent cell 
                    navmesh.TryGetParentSimpleContext<ICellGetter>(out var cell);

                    // Ignore null 
                    if (cell is null || cell.Record is null || cell.Record.Grid is null) continue;
                    if (navmesh is null) continue;

                    // Get parent worldspace
                    navmesh.TryGetParentSimpleContext<IWorldspaceGetter>(out var parent);
                    if (parent is null || parent.Record is null) continue;

                    // Worldspaces
                    if (worldspacesToMove.Contains(parent.Record.FormKey))
                    {
                        if (Settings.debug)
                            System.Console.WriteLine("Navmesh found in worldspace: " + navmesh.Record.FormKey);

                        // Get the relevant cells
                        if (!tamrielCellGrids.TryGetValue(cell.Record.Grid.Point, out var tamrielCellContext)) continue;
                        if (!originalCellGrid.TryGetValue(new Tuple<P2Int, FormKey>(cell.Record.Grid.Point, cell.Record.FormKey), out var originalCellContext)) continue;

                        // Get the original 
                        var original = originalCellContext.GetOrAddAsOverride(state.PatchMod);
                        if (original is null) continue;
                        var tamriel = tamrielCellContext.GetOrAddAsOverride(state.PatchMod);
                        if (tamriel is null) continue;

                        // Open/copy the Navmesh in the patch mod
                        var navmeshState = navmesh.GetOrAddAsOverride(state.PatchMod);
                        if (navmeshState is null) continue;


                        // Make a copy of the navmeshContext and place it in the right worldspace
                        if (Settings.copyNavmeshes)
                        {
                            var navcopy = navmeshState.Duplicate(state.PatchMod.GetNextFormKey());
                            if (navcopy.Data is null || navcopy.Data.Parent is null || original.Grid is null)
                            {
                                // Do nothing
                            }
                            else
                            {
                                // Create a Navmesh parent object
                                // WARNING: the Y comes first, then X!
                                P2Int16 p = new((short)original.Grid.Point.Y, (short)original.Grid.Point.X);

                                WorldspaceNavmeshParent tamrielasaparent = new() { Coordinates = p, Parent = Skyrim.Worldspace.Tamriel };
                                navcopy.Data.Parent = tamrielasaparent;
                            }

                            // Add the copy to Tamriel
                            tamriel.NavigationMeshes.Add(navcopy);

                            if (Settings.debug)
                                System.Console.WriteLine("     Navmesh copied, copy placed in Tamriel");
                        }

                        // Remove from the original worldspace cell and move to the corresponding Tamriel cell
                        // IMPORTANT NOTE: Currently causes CTDs due to NAVI 
                        else
                        {

                            var orgnavmeshes = original.NavigationMeshes;
                            IEnumerable<NavigationMesh> query =
                                from nav in orgnavmeshes
                                where nav.FormKey.Equals(navmesh.Record.FormKey)
                                select nav;
                            var navtoremove = query.FirstOrDefault();
                            if (navtoremove is null) continue;


                            if (navtoremove.Data is null || navtoremove.Data.Parent is null || original.Grid is null)
                            {
                                // Do nothing
                            }
                            else
                            {
                                // Create a Navmesh parent object
                                // WARNING: the Y comes first, then X!
                                P2Int16 p = new((short)original.Grid.Point.Y, (short)original.Grid.Point.X);

                                WorldspaceNavmeshParent tamrielasaparent = new() { Coordinates = p, Parent = Skyrim.Worldspace.Tamriel };
                                navtoremove.Data.Parent = tamrielasaparent;
                            }

                            // Remove from the original, add to Tamriel
                            original.NavigationMeshes.Remove(navtoremove);
                            tamriel.NavigationMeshes.Add(navtoremove);

                            if (Settings.debug)
                                System.Console.WriteLine("     Navmesh removed from parent, moved to Tamriel");

                        }

                        nbTotal++;
                    }
                }

                if (Settings.copyNavmeshes)
                    System.Console.WriteLine("Copied " + nbTotal + " Navmeshes into the Tamriel worldspace");
                else
                    System.Console.WriteLine("Moved " + nbTotal + " Navmeshes into the Tamriel worldspace");
            }
            

            System.Console.WriteLine("All done patching!");
        }
    }
}
