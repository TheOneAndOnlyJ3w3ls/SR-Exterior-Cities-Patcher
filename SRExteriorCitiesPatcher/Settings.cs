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
        [SettingName("Enable navmesh modification (Patch makers only))")]
        [Tooltip("Set to true to make copies rather than move navmeshes. False is unsupported/WIP")]
        public bool enableNavmeshEdit = false;

        [SettingName("Copy navmeshes (unticked is very experimental)")]
        [Tooltip("Set to true to make copies rather than move navmeshes. False is unsupported/WIP")]
        public bool copyNavmeshes = true;

        [SettingName("Ignore Occlusion")]
        [Tooltip("Set to true to ignore all occlusion planes")]
        public bool ignoreOcclusion = true;

        [SettingName("Debug")]
        [Tooltip("Activate all the debug messages")]
        public bool debug = false;
    }
}