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

namespace ContainersRespawnPatcher
{
    public class Program
    {
        // Dictionaries of containers
        //internal static Dictionary<P2Int, IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>> originalCellGrid = new();
        internal static Dictionary<Tuple<P2Int,FormKey>, IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>> originalCellGrid = new();

        internal static Dictionary<P2Int, IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>> tamrielCellGrids = new();

        internal static IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>? tamrielPersistentCellContext;
        internal static IFormLinkGetter<ICellGetter> tamrielPersistentCell = new FormLink<ICellGetter>(FormKey.Factory("000D74:Skyrim.esm"));

        public static Lazy<Settings> _settings = null!;
        public static Settings Settings => _settings.Value;

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings("Settings", "settings.json", out _settings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynthesisContainers.esp")
                .Run(args);
        }

        /*public static void CreateNewContainers(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            System.Console.WriteLine("Creating new 'No Respawn' containers!");

            // Counter
            int nbCont = 0;

            foreach (var containerGetter in state.LoadOrder.PriorityOrder.WinningOverrides<IContainerGetter>())
            {
                // Skip null container EditorID
                if (containerGetter.EditorID is null) continue;

                // If the EditorID of the container is found in the settings
                if (Settings.SafeContainersSettings.ContainerEditorIDs.Contains(containerGetter.EditorID))
                {
                    if (Settings.debug)
                        System.Console.WriteLine("Container found: " + containerGetter.EditorID);

                    // Check if the container already exists
                    state.LinkCache.TryResolve<IContainer>(containerGetter.EditorID + "_NoRespawn", out var existing);
                    if (existing is not null)
                    {
                        if (Settings.debug)
                            System.Console.WriteLine("   > Container _NoRespawn already exists: " + existing?.EditorID);

                        continue;
                    }

                    // Duplicate the record
                    Container contNew = state.PatchMod.Containers.DuplicateInAsNewRecord<Container, IContainerGetter>(containerGetter);

                    // Get the existing record
                    var contOld = state.PatchMod.Containers.GetOrAddAsOverride(containerGetter);

                    // Skip null
                    if (contOld.EditorID is null || contNew.EditorID is null) continue;

                    // If the container has a Respawn flag already, duplicate it and add no respawn
                    if (containerGetter.Flags.HasFlag(Container.Flag.Respawns))
                    {
                        // Name the new container NORESPAWN & remove the respawn flag 
                        contNew.EditorID = containerGetter.EditorID + "_NoRespawn";
                        contNew.Flags.SetFlag(Container.Flag.Respawns, false);

                        if (Settings.debug)
                            System.Console.WriteLine("   > Created new container: " + contNew.EditorID);

                        //Count
                        nbCont++;
                    }
                    // The container does not have the flag
                    else
                    {
                        // If the existing container has NoRespawn in their editorID, ignore it altogether
                        if (contOld.EditorID.Contains("norespawn", StringComparison.OrdinalIgnoreCase)) continue;

                        // Append the _NoRespawn text to the copy
                        contNew!.EditorID = contNew.EditorID + "_NoRespawn";

                        // Add the flag to the original container
                        contOld.Flags |= Container.Flag.Respawns;


                        if (Settings.debug)
                        {
                            System.Console.WriteLine("   > Created new container: " + contNew.EditorID);
                            System.Console.WriteLine("   > Added Respawn flag to container: " + contOld.EditorID);
                        }

                        // Count
                        nbCont++;
                    }

                    // Add the containers to the lists
                    containersRespawn.Add(contOld.FormKey, contOld);
                    containersNoRespawn.Add(contNew.FormKey, contNew);
                }
                else
                {
                    // Do nothing
                    //System.Console.WriteLine("Container not found: " + containerGetter.EditorID);
                }
            }

            System.Console.WriteLine("Created " + nbCont + " new containers!");
        }*/

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            // Create a link cache
            ILinkCache cache = state.LinkCache;


            /// Map Tamriel cells 
            System.Console.WriteLine("Mapping Tamriel");
            foreach (var cellContext in state.LoadOrder.ListedOrder.Cell().WinningContextOverrides(cache))
            {
                // Ignore null
                if (cellContext is null || cellContext.Record is null) continue;

                var cell = cellContext.Record;

                // Filter out interior cells
                if (cell.Flags.HasFlag(Cell.Flag.IsInteriorCell)) continue;

                // Filter out cells with no Grid
                if (cell.Grid is null) continue;

                // Filter out cells belonging to another worldspace than Tamriel
                if (!cellContext.TryGetParent<IWorldspaceGetter>(out var worldspace) || !worldspace.FormKey.Equals(Skyrim.Worldspace.Tamriel.FormKey)) continue;


                // Tamriel Persistent Cell
                if (tamrielPersistentCellContext is null 
                    && cellContext.Record.FormKey.Equals(tamrielPersistentCell.FormKey))
                {
                    tamrielPersistentCellContext = cellContext;

                    System.Console.WriteLine("Found Tamriel persistent cell! "+ tamrielPersistentCellContext.Record.FormKey);
                }
                else
                {
                    tamrielCellGrids.Add(cell.Grid.Point, cellContext);
                }
            }
            System.Console.WriteLine("Tamriel mapped");


            /// Map WhiterunWorld cells
            System.Console.WriteLine("Mapping WhiterunWorld");
            foreach (var cellContext in state.LoadOrder.ListedOrder.Cell().WinningContextOverrides(cache))
            {
                // Ignore null
                if (cellContext is null || cellContext.Record is null || cellContext.Parent is null) continue;

                var cell = cellContext.Record;

                // Filter out unwanted cells
                //if (cell.Flags.HasFlag(Cell.Flag.IsInteriorCell)) continue;
                if (cell.Grid is null) continue;

                // Ignore if the parent worldspace is null or not WhiterunWorld 
                if (!cellContext.TryGetParent<IWorldspaceGetter>(out var worldspace) || !worldspace.FormKey.Equals(Skyrim.Worldspace.WhiterunWorld.FormKey)) continue;


                // Add the cell context to the dictionary/map
                /*if(cell.Grid.Point.IsZero)
                {
                    System.Console.WriteLine("Zero found " + cell.Grid.Point + " / " + cell.FormKey);
                    System.Console.WriteLine("Zero cell is " + cell.Persistent.Count + " / " + cell.MajorFlags.HasFlag(Cell.MajorFlag.Persistent));


                    if (originalCellGrid.TryGetValue(new Tuple<P2Int,FormKey>(cell.Grid.Point,cell.FormKey), out var test))
                        System.Console.WriteLine("Zero added? " + test.Record.FormKey);

                }*/

                originalCellGrid.TryAdd(new Tuple<P2Int, FormKey>(cell.Grid.Point, cell.FormKey), cellContext);

            }
            System.Console.WriteLine("WhiterunWorld mapped!");


            // Counter
            int nbTotal = 0;
            int nbTempTotal = 0;
            int nbPersistTotal = 0;

            /// Check all placed objects 
            System.Console.WriteLine("Moving placed objects...");
            foreach (var placed in state.LoadOrder.PriorityOrder.PlacedObject().WinningContextOverrides(state.LinkCache))
            {
                // Get parent cell 
                placed.TryGetParentSimpleContext<ICellGetter>(out var cell);

                // Ignore null 
                if (cell is null || cell.Record is null || cell.Record.Grid is null) continue;
                if (placed is null) continue;

                // Ignore occlusion planes
                if (Settings.ignoreOcclusion && placed.Record.Base.FormKey.Equals(Skyrim.Static.PlaneMarker))
                {
                    if (Settings.debug)
                        System.Console.WriteLine("Occlusion plane ignored in worldspace!");
                    continue;
                }

                // Get the parent worldspace
                placed.TryGetParentSimpleContext<IWorldspaceGetter>(out var parent);
                if (parent is null || parent.Record is null) continue;

                // WhiterunWorld
                if (parent.Record.FormKey.Equals(Skyrim.Worldspace.WhiterunWorld.FormKey)) 
                {
                    if(Settings.debug)
                        System.Console.WriteLine("Object found in worldspace!");

                    // Get the relevant cells
                    if (!tamrielCellGrids.TryGetValue(cell.Record.Grid.Point, out var tamrielCellContext)) continue;
                    if (!originalCellGrid.TryGetValue(new Tuple<P2Int, FormKey>(cell.Record.Grid.Point, cell.Record.FormKey), out var originalCellContext)) continue;

                    // Get the original 
                    var original = originalCellContext.GetOrAddAsOverride(state.PatchMod);
                    if (original is null) continue;


                    // Open/copy the PlacedObject in the patch mod
                    var placedState = placed.GetOrAddAsOverride(state.PatchMod);

                    // Move persistent objects to the Tamriel Persistent cell
                    if (original.Persistent.Contains(placedState))
                    {
                        if (tamrielPersistentCellContext is null) continue;

                        // Get the Tamriel Persistent cell
                        var tamPersistCell = tamrielPersistentCellContext.GetOrAddAsOverride(state.PatchMod);
                        if (tamPersistCell is null) continue;

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
                        if (tamriel is null ) continue;

                        // Remove from the original worldspace cell and move to the corresponding Tamriel cell
                        original.Temporary.Remove(placedState);
                        tamriel.Temporary.Add(placedState);
                        tamriel.Location.SetTo(original.Location);

                        // Count
                        nbTempTotal++;
                        nbTotal++;

                        if (Settings.debug)
                            System.Console.WriteLine("Temporary object moved from " + parent.Record.EditorID + " " + original.Grid?.Point.ToString() + " to " + tamriel.Grid?.Point.ToString());
                    }
                }
            }
            System.Console.WriteLine("Moved " + nbTotal + " objects (" + nbPersistTotal + " peristent + " + nbTempTotal + " temporary objects)");

            // Reset counters
            nbTotal = 0;
            nbTempTotal = 0;
            nbPersistTotal = 0;

            /// Check all placed NPCs 
            System.Console.WriteLine("Moving placed NPCs...");
            foreach (var placed in state.LoadOrder.PriorityOrder.PlacedNpc().WinningContextOverrides(state.LinkCache))
            {
                // Get parent cell 
                placed.TryGetParentSimpleContext<ICellGetter>(out var cell);

                // Ignore null 
                if (cell is null || cell.Record is null || cell.Record.Grid is null) continue;
                if (placed is null) continue;

                // Get the parent worldspace
                placed.TryGetParentSimpleContext<IWorldspaceGetter>(out var parent);
                if (parent is null || parent.Record is null) continue;

                // WhiterunWorld
                if (parent.Record.FormKey.Equals(Skyrim.Worldspace.WhiterunWorld.FormKey))
                {
                    if (Settings.debug)
                        System.Console.WriteLine("NPC found in worldspace!");

                    // Get the relevant cells
                    if (!tamrielCellGrids.TryGetValue(cell.Record.Grid.Point, out var tamrielCellContext)) continue;
                    if (!originalCellGrid.TryGetValue(new Tuple<P2Int, FormKey>(cell.Record.Grid.Point, cell.Record.FormKey), out var originalCellContext)) continue;

                    // Get the original 
                    var original = originalCellContext.GetOrAddAsOverride(state.PatchMod);
                    if (original is null) continue;


                    // Open/copy the PlacedObject in the patch mod
                    var placedState = placed.GetOrAddAsOverride(state.PatchMod);

                    // Move persistent objects to the Tamriel Persistent cell
                    if (original.Persistent.Contains(placedState))
                    {
                        if (tamrielPersistentCellContext is null) continue;

                        // Get the Tamriel Persistent cell
                        var tamPersistCell = tamrielPersistentCellContext.GetOrAddAsOverride(state.PatchMod);
                        if (tamPersistCell is null) continue;

                        // Remove from the original worldspace cell and move to the Tamriel Persistent cell
                        original.Persistent.Remove(placedState);
                        tamPersistCell.Persistent.Add(placedState);

                        // Count
                        nbPersistTotal++;
                        nbTotal++;

                        if (Settings.debug)
                            System.Console.WriteLine("Persistent NPC moved from " + parent.Record.EditorID + " " + original.Grid?.Point.ToString() + " to " + tamPersistCell.FormKey);
                    }

                    // Move the temporary object to the Tamriel worldspace
                    if (original.Temporary.Contains(placedState))
                    {
                        var tamriel = tamrielCellContext.GetOrAddAsOverride(state.PatchMod);
                        if (tamriel is null) continue;

                        // Remove from the original worldspace cell and move to the corresponding Tamriel cell
                        original.Temporary.Remove(placedState);
                        tamriel.Temporary.Add(placedState);
                        tamriel.Location.SetTo(original.Location);

                        // Count
                        nbTempTotal++;
                        nbTotal++;

                        if (Settings.debug)
                            System.Console.WriteLine("NPC moved from " + parent.Record.EditorID + " " + original.Grid?.Point.ToString() + " to Tamriel " + tamriel.Grid?.Point.ToString());
                    }
                }
            }
            System.Console.WriteLine("Moved " + nbTotal + " NPCs (" + nbPersistTotal + " peristent + " + nbTempTotal + " temporary NPCs)");


            /// Navmeshes

                System.Console.WriteLine("All done patching!");
        }
    }
}
