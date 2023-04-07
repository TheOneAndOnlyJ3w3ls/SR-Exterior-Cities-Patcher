using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainersRespawnPatcher
{
    public class Cells
    {
        [SettingName("EditorIDs of Cells without respawn")]
        [Tooltip("All the cells that should not have respawning containers (player homes, ...)")]
        public List<string> CellNoRespawnEditorIDs = new ()
        {
            "WhiterunJorrvaskrBasement",
            "WhiterunJorrvaskr",
            "WinterholdCollegeHallofAttainment",
            "WinterholdCollegeArchMageQuarters",
            "RiftenThievesGuildHeadquarters",
            "SolitudeProudspireManor",
            "WhiterunBreezehome",
            "WindhelmHjerim",
            "RiftenHoneyside",
            "MarkarthVlindrelHall",
            "DLC1DawnguardHQ01",
            "DLC1VampireCastleGuildhall",
            "BYOHHouse1Falkreath",
            "BYOHHouse1FalkreathBasement",
            "BYOHHouse2Hjaalmarch",
            "BYOHHouse2HjaalmarchBasement",
            "BYOHHouse3Pale",
            "BYOHHouse3PaleBasement",
            "DLC2RRSeverinHouse"
        };
}

    public class Settings
    {
        public IFormLinkGetter<IWorldspaceGetter> WhiterunWorld = new FormLink<IWorldspaceGetter>(FormKey.Factory("01A26F:Skyrim.esm"));
        public IFormLinkGetter<IWorldspaceGetter> Tamriel = new FormLink<IWorldspaceGetter>(FormKey.Factory("00003c:Skyrim.esm"));

        public Cells CellsNotRespawningSettings { get; set; } = new();

        [SettingName("Debug")]
        [Tooltip("Activate all the debug messages")]
        public bool debug = false;
    }
}