using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainersRespawnPatcher
{

    public class Settings
    {
        /*public IFormLinkGetter<IWorldspaceGetter> WhiterunWorld = new FormLink<IWorldspaceGetter>(FormKey.Factory("01A26F:Skyrim.esm"));
        public IFormLinkGetter<IWorldspaceGetter> Tamriel = new FormLink<IWorldspaceGetter>(FormKey.Factory("00003c:Skyrim.esm"));*/


        [SettingName("Copy navmeshes (experimental)")]
        [Tooltip("Set to true to make copies rather than move navmeshes")]
        public bool copyNavmeshes = true;

        [SettingName("Ignore Occlusion")]
        [Tooltip("Set to true to ignore all occlusion planes")]
        public bool ignoreOcclusion = true;

        [SettingName("Debug")]
        [Tooltip("Activate all the debug messages")]
        public bool debug = false;
    }
}