#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [ElementEditor(typeof(NoiseFilter))]
    public class NoiseFilterEditor : ReorderableListComponentEditor
    {
        private NoiseFilter _noiseFilter;

        private NoiseSettingsGUI _noiseSettingsGUI;

        private NoiseSettingsGUI NoiseSettingsGUI
        {
            get
            {
                if (_noiseSettingsGUI == null)
                {
                    _noiseSettingsGUI = new NoiseSettingsGUI(_noiseFilter.NoiseSettings);
                }

                return _noiseSettingsGUI;
            }
        }

        public override void OnEnable()
        {
            _noiseFilter = (NoiseFilter)Target;
        }

        public override void OnGUI(Rect rect, int index)
        {
            if (index != 0)
            {
                _noiseFilter.BlendMode = (BlendMode)CustomEditorGUI.EnumPopup(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Blend Mode"), _noiseFilter.BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            CreateNoiseSettingsIfNecessary();

            NoiseSettingsGUI.OnGUI(rect);
        }

        public override float GetElementHeight(int index)
        {
            CreateNoiseSettingsIfNecessary();

            float height = 0;

            if (index != 0)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            if (_noiseFilter.NoiseSettings.ShowNoisePreviewTexture)
            {
                height += 256f + EditorGUIUtility.singleLineHeight * 2;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;

            if (_noiseFilter.NoiseSettings.ShowNoiseTransformSettings)
            {
                height += EditorGUIUtility.singleLineHeight * 7;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;

            if (_noiseFilter.NoiseSettings.ShowNoiseTypeSettings)
            {
                height += EditorGUIUtility.singleLineHeight * 12;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            return height;
        }

        private void CreateNoiseSettingsIfNecessary()
        {
            if (_noiseFilter.NoiseSettings == null)
            {
                _noiseFilter.NoiseSettings = new NoiseSettings();
            }
        }
    }
}
#endif
