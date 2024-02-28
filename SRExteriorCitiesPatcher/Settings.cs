using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRExteriorCitiesPatcher
{

    public class Settings
    {
        [SettingName("Ignore Occlusion")]
        [Tooltip("Set to true to ignore all occlusion planes")]
        public bool ignoreOcclusion = true;

        [SettingName("PATCH MAKER ONLY: Enable navmesh modification")]
        [Tooltip("Set to true to make navmeshes edits. Recommended for patch makers only, will need a run in the CK")]
        public bool enableNavmeshEdit = false;

        [SettingName("PATCH MAKER ONLY: Move SREX and patches navmeshes down (experimental)")]
        [Tooltip("Set to true to move all SREX + patches navmeshes down to -30000.")]
        public bool moveNavmeshes = false;

        [SettingName("Debug")]
        [Tooltip("Activate all the debug messages")]
        public bool debug = false;
    }
}