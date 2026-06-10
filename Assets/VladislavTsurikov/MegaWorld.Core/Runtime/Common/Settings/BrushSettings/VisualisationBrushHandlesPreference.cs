using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.AdvancedBrushSettings
{
    [Name("Visualisation Brush Handles")]
    public class VisualisationBrushHandlesPreference : PreferenceSettings
    {
        [Name("Circle Color")]
        public Color CircleColor = new(0.2f, 0.5f, 0.7f, 1);
    }
}
