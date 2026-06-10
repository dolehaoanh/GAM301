#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.AdvancedBrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using AdvancedBrushSettingsElement =
    VladislavTsurikov.MegaWorld.Runtime.Common.Settings.AdvancedBrushSettings.AdvancedBrushSettings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.BrushSettings
{
    [Serializable]
    public class BrushJitterSettingsEditor
    {
        [NonSerialized]
        private readonly GUIContent _brushJitter = new("Jitter", "Control brush stroke randomness.");

        [NonSerialized]
        private readonly GUIContent _brushScatter = new("Brush Scatter", "Randomize brush position by an offset.");

        [NonSerialized]
        private readonly GUIContent _brushSize = new("Brush Size",
            "Selected prototypes will only spawn in this range around the center of Brush.");

        public void OnGUI(AdvancedBrushSettingsElement brush, BrushJitterSettings jitter)
        {
            brush.BrushSize = CustomEditorGUILayout.Slider(_brushSize, brush.BrushSize, 0.1f,
                PreferenceElementSingleton<BrushPreferenceSettings>.Instance.MaxBrushSize);

            jitter.BrushSizeJitter = CustomEditorGUILayout.Slider(_brushJitter, jitter.BrushSizeJitter, 0f, 1f);

            //CustomEditorGUILayout.Separator();

            jitter.BrushScatter = CustomEditorGUILayout.Slider(_brushScatter, jitter.BrushScatter, 0f, 1f);
            jitter.BrushScatterJitter = CustomEditorGUILayout.Slider(_brushJitter, jitter.BrushScatterJitter, 0f, 1f);
        }
    }
}
#endif
