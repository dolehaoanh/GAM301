using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings
{
    [Name("Overlap Check Settings")]
    public class OverlapCheckSettingsPreference : PreferenceSettings
    {
        [Name("Multiply Find Size")]
        [Tooltip(
            "If you increase the Overlap Shape, but the object spawns inside another Overlap Shape, this means that the algorithm did not find this object, increase this parameter, but this will slow down the spawn.")]
        public float MultiplyFindSize = 1f;
    }
}
