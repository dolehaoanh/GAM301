using VladislavTsurikov.CustomInspector.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.AdvancedBrushSettings
{
    [Name("Brush")]
    public class BrushPreferenceSettings : PreferenceSettings
    {
        [Name("Max Brush Size")]
        [Min(0.5f)]
        public float MaxBrushSize = 200;
    }
}
