using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace SRExteriorCitiesPatcher
{

    public class Settings
    {
        [SettingName("Check Load order correctness")]
        [Tooltip("Set to true to do a check for the order of your SREX patches")]
        public bool checkLO = true;


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