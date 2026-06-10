using VladislavTsurikov.CustomInspector.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [Name("Scatter")]
    public class ScatterPreferenceSettings : PreferenceSettings
    {
        [Name("Max Checks")]
        [Min(1)]
        public int MaxChecks = 100;
    }
}
