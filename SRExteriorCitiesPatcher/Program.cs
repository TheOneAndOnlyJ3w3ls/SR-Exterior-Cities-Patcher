using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using Noggog;
using Mutagen.Bethesda.Plugins.Records;
using DynamicData;

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

        internal static Dictionary<FormKey, IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>> worldspacesPersistentCellContexts = new();

        // SR Exterior Cities main plugin
        internal static ModKey srexMain;
        internal static bool srexModExists = false;
        internal static HashSet<FormKey> SREXMainFormIDs = new();

        // Doors
        internal static Dictionary<FormKey, IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>> srexDoors = new();

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
            FormKey.Factory("0F795E:Skyrim.esm"), // Rften canal gate 1
            FormKey.Factory("0E8289:Skyrim.esm"), // Rften canal gate 2
            FormKey.Factory("055FCA:Skyrim.esm"), // Windhelm main gate
            FormKey.Factory("0A17D3:Skyrim.esm"), // Windhelm main gate 2
            FormKey.Factory("101D9D:Skyrim.esm"), // Solitude main gate
            FormKey.Factory("037F1B:Skyrim.esm"), // Solitude main gate 2
            FormKey.Factory("01C38B:Skyrim.esm"), // Markarth main gate
            FormKey.Factory("0383C9:Skyrim.esm"), // Whiterun main gateway
            FormKey.Factory("01B1F3:Skyrim.esm"), // Whiterun main gate
        };


        private static readonly List<FormKey> persistentWorldspaceCells = new()
        {
            FormKey.Factory("016BE0:Skyrim.esm"), // Riften
            FormKey.Factory("01691F:Skyrim.esm"), // Windhelm
            FormKey.Factory("037EE6:Skyrim.esm"), // Solitude
            FormKey.Factory("01A270:Skyrim.esm"), // Whiterun
            FormKey.Factory("016E02:Skyrim.esm"), // Markarth
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


                if (persistentWorldspaceCells.Contains(cell.Record.FormKey))
                {
                    if(!worldspacesPersistentCellContexts.TryGetValue(cell.Record.FormKey, out var originalCellContext)) return;

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
                }
                else
                {
                    // Get the relevant cells
                    if (!tamrielCellGrids.TryGetValue(cell.Record.Grid.Point, out var tamrielCellContext)) return;
                    if (!originalCellGrid.TryGetValue(new Tuple<P2Int, FormKey>(cell.Record.Grid.Point, cell.Record.FormKey), out var originalCellContext)) return;

                    // Get the original 
                    var original = originalCellContext.GetOrAddAsOverride(state.PatchMod);
                    if (original is null) return;

                    // Open/copy the PlacedObject in the patch mod
                    var placedState = placed.GetOrAddAsOverride(state.PatchMod);



                    // Move the temporary object to the Tamriel worldspace
                    if (original.Temporary.Contains(placedState))
                    {
                        var tamriel = tamrielCellContext.GetOrAddAsOverride(state.PatchMod);
                        if (tamriel is null) return;

                        // Remove from the original worldspace cell and move to the corresponding Tamriel cell
                        original.Temporary.Remove(placedState);
                        tamriel.Temporary.Add(placedState);

                        // Count
                        nbTempTotal++;
                        nbTotal++;

                        if (Settings.debug)
                            System.Console.WriteLine("Temporary object moved from " + parent.Record.EditorID + " " + original.Grid?.Point.ToString() + " to " + tamriel.Grid?.Point.ToString());
                    }
                }
            }

            // Show progress every 1000 records moved
            if (nbTotal % 1000 == 0 && nbTotal > 0)
                System.Console.WriteLine("Moved " + nbTotal + " objects...");
        }


        /**
         * Alters the door placement, flags and enableparent accordingly
         */
        internal static void DoDoorDisplacement(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter> door)
        {
            // Only update the placement
            srexDoors.TryGetValue(door.Record.FormKey, out var value);
            if (value is null) return;

            Placement placement = new();
            if (door.Record.Placement is not null)
                placement = new()
                {
                    Position = door.Record.Placement.Position,
                    Rotation = door.Record.Placement.Rotation
                };


            EnableParent enableParent = new();
            if (door.Record.EnableParent is not null)
                enableParent = new()
                {
                    Flags = door.Record.EnableParent.Flags,
                    Reference = door.Record.EnableParent.Reference.AsSetter(),
                    Versioning = door.Record.EnableParent.Versioning
                };

            int majorFlags = door.Record.MajorRecordFlagsRaw;
            SkyrimMajorRecord.SkyrimMajorRecordFlag flags = door.Record.SkyrimMajorRecordFlags;
            IFormLink<ILocationRecordGetter> location = door.Record.LocationReference.AsSetter();


            var placedState = value.GetOrAddAsOverride(state.PatchMod);
            if (placedState is null) return;

            // Only alter the placement, flags, Enable Parent, Flags and Location
            if (door.Record.Placement is not null)
                placedState.Placement = placement;

            if (door.Record.EnableParent is not null)
                placedState.EnableParent = enableParent;

            placedState.MajorRecordFlagsRaw = majorFlags;

            placedState.SkyrimMajorRecordFlags = flags;

            if (!location.IsNull)
                placedState.LocationReference = location.AsNullable();

            if (Settings.debug)
            {
                System.Console.WriteLine("Door found, already modified by SREX");
            }
        }


        internal static void MoveNavmeshDown(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, IModContext<ISkyrimMod, ISkyrimModGetter, INavigationMesh, INavigationMeshGetter> navmeshContext)
        {
            // Get parent cell 
            navmeshContext.TryGetParentSimpleContext<ICellGetter>(out var cell);

            // Ignore null 
            if (cell is null || cell.Record is null || cell.Record.Grid is null) return;
            if (navmeshContext is null) return;

            // Get parent worldspace
            navmeshContext.TryGetParentSimpleContext<IWorldspaceGetter>(out var parent);
            if (parent is null || parent.Record is null) return;

            // Worldspaces
            if (parent.Record.FormKey.Equals(Skyrim.Worldspace.Tamriel.FormKey))
            {
                if (Settings.debug)
                    System.Console.WriteLine("Navmesh found in worldspace: " + navmeshContext.Record.FormKey);


                if (navmeshContext.Record.Data == null) return;

                // Find the lowest vertices in the navmeshContext
                float lowest = navmeshContext.Record.Data.Vertices[0].Z;
                foreach (var NavVertex in navmeshContext.Record.Data.Vertices)
                {
                    lowest = (NavVertex.Z < lowest) ? NavVertex.Z : lowest;
                }
                lowest += 30000;


                // Open/copy the Navmesh in the patch mod
                var navmeshState = navmeshContext.GetOrAddAsOverride(state.PatchMod);
                if (navmeshState is null) return;

                // Lower all vertices
                for (int i = 0; i < navmeshContext.Record.Data.Vertices.Count; i++)
                {
                    navmeshState.Data ??= new();

                    var oldPoint = navmeshState.Data.Vertices[i];
                    P3Float newPoint = new(oldPoint.X, oldPoint.Y, oldPoint.Z-lowest);

                    navmeshState.Data.Vertices[i] = newPoint;
                }
            }
        }


        /**
         * Fill in dictionaries of the valid cells, both in the city Worldspaces and the corresponding Tamriel cells
         */
        internal static void DoWorldspaceMapping(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach (var cellContext in state.LoadOrder.PriorityOrder.Cell().WinningContextOverrides(state.LinkCache))
            {
                // Ignore null
                if (cellContext is null || cellContext.Record is null) continue;

                var cell = cellContext.Record;

                // Filter out interior cells
                if (cell.Flags.HasFlag(Cell.Flag.IsInteriorCell)) continue;

                // Handle persistent cells in the city worldspaces
                if (persistentWorldspaceCells.Contains(cell.FormKey)) //cell.MajorFlags.HasFlag(Cell.MajorFlag.Persistent)
                {
                    worldspacesPersistentCellContexts.TryAdd(cell.FormKey, cellContext);
                    continue;
                }

                // Tamriel Persistent Cell
                if (tamrielPersistentCellContext is null
                    && cellContext.Record.FormKey.Equals(tamrielPersistentCell.FormKey))
                {
                    tamrielPersistentCellContext = cellContext;

                    System.Console.WriteLine("Found Tamriel persistent cell! " + tamrielPersistentCellContext.Record.FormKey);
                    continue;
                }

                // Filter out cells with no Grid
                if (cell.Grid is null) continue;

                // Try getting the worldspace
                if (!cellContext.TryGetParent<IWorldspaceGetter>(out var worldspace)) continue;

                // Tamriel worldspace
                if (worldspace.FormKey.Equals(Skyrim.Worldspace.Tamriel.FormKey))
                {
                    bool test = tamrielCellGrids.TryAdd(cell.Grid.Point, cellContext);

                    if (Settings.debug && !test)
                        System.Console.WriteLine("Duplicate cell found at " + cell.Grid.Point.X + "," + cell.Grid.Point.Y);
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
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynthesisSREX.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {

            /* --------------------------------------------------- \
            |                   INITIALISATION                     |
            \ --------------------------------------------------- */

            /// Variables and initialisation
            // Create a link cache
            ILinkCache cache = state.LinkCache;


            // Try to get the main mod
            if (state.LoadOrder.TryGetValue("SR Exterior Cities.esp", out var srexMainTmp))
            {
                srexMain = srexMainTmp.ModKey;
                System.Console.WriteLine("Main SE Exterior plugin detected!");

                srexModExists = true;

                // Add it as master
                MasterReference m = new() { Master = srexMain, FileSize = 0 };
                state.PatchMod.ModHeader.MasterReferences.Add(m);
            }
            else
            {
                System.Console.WriteLine("ERROR: Main SE Exterior plugin not detected!");
                return;
            }

            // Should not happen to be null here
            if (srexMainTmp is null || srexMainTmp.Mod is null) return;

            // Create a list of formkeys from SREX main
            HashSet<FormKey> formKeys = new HashSet<FormKey>();
            foreach (var fl  in srexMainTmp.Mod.EnumerateFormLinks())
            {
                formKeys.Add(fl.FormKey);
            }

            if (Settings.debug)
                System.Console.WriteLine("Found formkeys in srex: " + formKeys.Count);


            // Make sure SREX main plugin is set as master
            foreach (var fl in srexMainTmp.Mod.Statics)
                state.PatchMod.Statics.GetOrAddAsOverride(fl);

            /// Mapping out all relevant cells in Tamriel and other worldspaces 
            System.Console.WriteLine("Mapping out the worldspaces...");
            DoWorldspaceMapping(state);
            System.Console.WriteLine("All worldspaces mapped!");


            /* --------------------------------------------------- \
            |             FORM LOADORDERS TO CONSIDER              |
            \ --------------------------------------------------- */


            IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>> loadOrderToConsiderForObjects;
            IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>> loadOrderToConsiderForNPCs;
            IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, IAPlacedTrap, IAPlacedTrapGetter>> loadOrderToConsiderForHazards;
            
            var modsWithoutSREXPatches = state.LoadOrder.PriorityOrder.Where(x => !x.ModKey.FileName.String.Contains("SREX AIO", StringComparison.Ordinal));

            loadOrderToConsiderForObjects = modsWithoutSREXPatches.PlacedObject().WinningContextOverrides(state.LinkCache);
            loadOrderToConsiderForNPCs = modsWithoutSREXPatches.PlacedNpc().WinningContextOverrides(state.LinkCache);
            loadOrderToConsiderForHazards = modsWithoutSREXPatches.APlacedTrap().WinningContextOverrides(state.LinkCache);

            // Exclude the main plugin too
            loadOrderToConsiderForObjects = state.LoadOrder.PriorityOrder
                .Where(x => !x.ModKey.FileName.String.Contains("SREX AIO", StringComparison.Ordinal) && !x.ModKey.Equals(srexMain))
                .PlacedObject().WinningContextOverrides(state.LinkCache);


            /* --------------------------------------------------- \
            |                    MOVE OBJECTS                      |
            \ --------------------------------------------------- */

            /// OBJECTS
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

                // Ignore blacklisted doors
                if (doorsToExclude.Contains(placed.Record.FormKey)) continue;

                // If SREX is winning the conflict
                // if (placed.ModKey.Equals(srexMain))
                if (formKeys.Contains(placed.Record.FormKey)) 
                {
                    // If the second to last winning mod is vanilla
                    if(vanillaModKeys.Contains(placed.ModKey))
                    {
                        // Don't move it
                    }
                    // If the second to last is NOT vanilla (aka modified by another mod)
                    else
                    {
                        // Move
                        DoSimpleMove(state, placed);
                    }
                }
                // Else if the winning mod is Vanilla (aka modified by another mod)
                else if (vanillaModKeys.Contains(placed.ModKey))
                {
                    // Don't move it
                }
                // Else, the winning mod is NOT Vanilla
                else
                {
                    // Move it
                    DoSimpleMove(state, placed);
                }
            }
            System.Console.WriteLine("All done moving objects!");
            System.Console.WriteLine("Moved " + nbTotal + " objects (" + nbPersistTotal + " persistent + " + nbTempTotal + " temporary objects)");

            // Reset counters
            nbTotal = 0;
            nbTempTotal = 0;
            nbPersistTotal = 0;

            /// NPCs
            System.Console.WriteLine("Moving NPCs...");
            foreach (var placed in loadOrderToConsiderForNPCs)
            {

                // If SREX is winning the conflict
                if (formKeys.Contains(placed.Record.FormKey))
                {
                    // If the second to last winning mod is vanilla
                    if (vanillaModKeys.Contains(placed.ModKey))
                    {
                        // Don't move it
                    }
                    // If the second to last is NOT vanilla (aka modified by another mod)
                    else
                    {
                        // Move
                        DoSimpleMove(state, placed);
                    }
                }
                // Else if the winning mod is Vanilla (aka modified by another mod)
                else if (vanillaModKeys.Contains(placed.ModKey))
                {
                    // Don't move it
                }
                // Else, the winning mod is NOT Vanilla
                else
                {
                    // Move it
                    DoSimpleMove(state, placed);
                }
            }
            System.Console.WriteLine("All done moving NPCs!");
            System.Console.WriteLine("Moved " + nbTotal + " NPCs (" + nbPersistTotal + " persistent + " + nbTempTotal + " temporary NPCs)");


            // Reset counters
            nbTotal = 0;
            nbTempTotal = 0;
            nbPersistTotal = 0;


            /// HAZARDS (traps etc)
            System.Console.WriteLine("Moving Hazards...");
            foreach (var placed in loadOrderToConsiderForHazards)
            {
                // If SREX is winning the conflict
                if (formKeys.Contains(placed.Record.FormKey))
                {
                    // If the second to last winning mod is vanilla
                    if (vanillaModKeys.Contains(placed.ModKey))
                    {
                        // Don't move it
                    }
                    // If the second to last is NOT vanilla (aka modified by another mod)
                    else
                    {
                        // Move
                        DoSimpleMove(state, placed);
                    }
                }
                // Else if the winning mod is Vanilla (aka modified by another mod)
                else if (vanillaModKeys.Contains(placed.ModKey))
                {
                    // Don't move it
                }
                // Else, the winning mod is NOT Vanilla
                else
                {
                    // Move it
                    DoSimpleMove(state, placed);
                }
            }
            System.Console.WriteLine("All done moving Hazards!");
            System.Console.WriteLine("Moved " + nbTotal + " hazards (" + nbPersistTotal + " persistent + " + nbTempTotal + " temporary hazards)");


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
                NavmeshMapping mapping = new();

                Dictionary<FormKey, IModContext<ISkyrimMod, ISkyrimModGetter, INavigationMesh, INavigationMeshGetter>> SREXGridNavmeshes = new();

                // Find all navmeshes by the SREX formIDs
                foreach (var navmesh in state.LoadOrder.PriorityOrder.Where(x => x.ModKey.Equals(srexMain)).NavigationMesh().WinningContextOverrides(state.LinkCache))
                {
                    // Get parent worldspace
                    navmesh.TryGetParentSimpleContext<IWorldspaceGetter>(out var parent);

                    // Ignore it the parent is null or if it is not Tamriel
                    if (parent is null || parent.Record is null || !parent.Record.FormKey.Equals(Skyrim.Worldspace.Tamriel.FormKey)) continue;

                    SREXGridNavmeshes.TryAdd(navmesh.Record.FormKey, navmesh);
                }


                if(Settings.moveNavmeshes)
                {
                    // Only consider winning overrides that are in SREX or a patch
                    var loadorder = state.LoadOrder.PriorityOrder.NavigationMesh().WinningContextOverrides(state.LinkCache)
                                        .Where(x => /*x.ModKey.Equals(srexMain) || vanillaModKeys.Contains(x.ModKey)*/
                                                    x.ModKey.Equals(srexMain) || x.ModKey.FileName.String.Contains("SREX AIO", StringComparison.Ordinal));


                    Dictionary<Tuple<P2Int, FormKey>, INavigationMesh> originalNavmeshes = new();
                    Dictionary<Tuple<P2Int, FormKey>, INavigationMesh> srexNavmeshes = new();


                    System.Console.WriteLine("Moving existing navmeshes from SREX Main under the map...");
                    // Move SR Exterior Cities navmeshes down
                    foreach (var navmesh in loadorder)
                    {
                        // Determine if the placed object should be moved
                        var contexts = state.LinkCache.ResolveAllContexts<IPlaced, IPlacedGetter>(navmesh.Record.FormKey);
                        var modkeys = contexts.Select(x => x.ModKey).Reverse();

                        // Don't move the vanilla ones
                        if(vanillaModKeys.Contains(modkeys.First()))
                        {
                            continue;
                        }

                        MoveNavmeshDown(state, navmesh);
                    }
                    System.Console.WriteLine("Done moving!");
                }
                


                // Create a new navmesh from the interior worldspace (or move it)
                var loadorderForNavmeshes = state.LoadOrder.PriorityOrder.NavigationMesh().WinningContextOverrides(state.LinkCache)
                    .Where(x => !x.ModKey.Equals(srexMain)  && !x.ModKey.FileName.String.Contains("SREX AIO", StringComparison.Ordinal));
                foreach (var navmesh in loadorderForNavmeshes)
                //foreach (var navmesh in state.LoadOrder.PriorityOrder.NavigationMesh().WinningContextOverrides(state.LinkCache))
                {
                    // Get parent cell 
                    navmesh.TryGetParentSimpleContext<ICellGetter>(out var cell);

                    // Ignore null 
                    if (cell is null || cell.Record is null || cell.Record.Grid is null) continue;
                    if (navmesh is null) continue;

                    // Get parent worldspace
                    navmesh.TryGetParentSimpleContext<IWorldspaceGetter>(out var parent);
                    if (parent is null || parent.Record is null) continue;

                    // If the navmesh is in a worldspace to move
                    if (worldspacesToMove.Contains(parent.Record.FormKey))
                    {
                        if (Settings.debug)
                            System.Console.WriteLine("Navmesh found in worldspace: " + navmesh.Record.FormKey);

                        
                        // If the second to last winning mod is vanilla, ignore that navmesh
                        if (vanillaModKeys.Contains(navmesh.ModKey)) continue;

                        // Overwrite SREX navmesh if it exists in the mapping
                        if (mapping.NavmeshMap.TryGetValue(navmesh.Record.FormKey, out var newFormKeyFromMapping))
                        {
                            // Get the SREX navmesh context
                            if (!SREXGridNavmeshes.TryGetValue(newFormKeyFromMapping, out var navmeshContext)) continue;

                            // Get the SREX navmesh to overwrite
                            var navmeshOverwrite = navmeshContext.GetOrAddAsOverride(state.PatchMod);

                            if (navmeshOverwrite is not null && navmeshOverwrite.Data is not null)
                            {
                                var x = navmesh.Record.DeepCopy();

                                var parentWorldspace = navmeshOverwrite.Data.Parent;

                                navmeshOverwrite.Data = x.Data;
                                if (navmeshOverwrite.Data is not null) 
                                    navmeshOverwrite.Data.Parent = parentWorldspace;

                                navmeshOverwrite.EditorID = x.EditorID;
                                navmeshOverwrite.MajorFlags = x.MajorFlags;
                                navmeshOverwrite.NNAM = x.NNAM;
                                navmeshOverwrite.ONAM = x.ONAM;
                                navmeshOverwrite.PNAM = x.PNAM;
                                navmeshOverwrite.SkyrimMajorRecordFlags = x.SkyrimMajorRecordFlags;


                                //if (Settings.debug)
                                    System.Console.WriteLine("     Navmesh: " + navmeshOverwrite.FormKey + " overwritten by " + navmesh.Record.FormKey);
                            }
                        }

                        // Otherwise, copy the navmesh as new record
                        else
                        {
                            // Open/copy the Navmesh in the patch mod
                            var navmeshState = navmesh.GetOrAddAsOverride(state.PatchMod);
                            if (navmeshState is null) continue;

                            // Make a copy of the navmeshContext and place it in the right worldspace
                            var navcopy = navmeshState.Duplicate(state.PatchMod.GetNextFormKey());
                            if (navcopy is null || navcopy.Data is null || navcopy.Data.Parent is null)
                            {
                                // Do nothing if null
                                continue;
                            }

                            // Create a Navmesh parent object
                            // WARNING: the Y comes first, then X!
                            P2Int16 p = new((short)cell.Record.Grid.Point.Y, (short)cell.Record.Grid.Point.X);

                            WorldspaceNavmeshParent tamrielasaparent = new() { Coordinates = p, Parent = Skyrim.Worldspace.Tamriel };
                            navcopy.Data.Parent = tamrielasaparent;


                            // Get the relevant cells
                            if (!tamrielCellGrids.TryGetValue(cell.Record.Grid.Point, out var tamrielCellContext)) continue;

                            var tamriel = tamrielCellContext.GetOrAddAsOverride(state.PatchMod);
                            if (tamriel is null) continue;

                            // Add the copy to Tamriel
                            tamriel.NavigationMeshes.Add(navcopy);

                            if (Settings.debug)
                                System.Console.WriteLine("     Navmesh copied, copy placed in Tamriel");

                        }

                        nbTotal++;
                    }
                }

                if (Settings.enableNavmeshEdit)
                    System.Console.WriteLine("Copied " + nbTotal + " Navmeshes into the Tamriel worldspace");

            }
            

            System.Console.WriteLine("All done patching!");
        }
    }
}
