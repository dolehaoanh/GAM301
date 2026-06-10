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
    [ElementEditor(typeof(HeightNoiseFilter))]
    public class HeightNoiseFilterEditor : ReorderableListComponentEditor
    {
        private HeightNoiseFilter _heightNoiseFilter;

        private NoiseSettingsGUI _mNoiseSettingsGUI;

        private NoiseSettingsGUI noiseSettingsGUI
        {
            get
            {
                if (_mNoiseSettingsGUI == null)
                {
                    _mNoiseSettingsGUI = new NoiseSettingsGUI(_heightNoiseFilter.NoiseSettings);
                }

                return _mNoiseSettingsGUI;
            }
        }

        public override void OnEnable()
        {
            _heightNoiseFilter = (HeightNoiseFilter)Target;
        }

        public override void OnGUI(Rect rect, int index)
        {
            if (index != 0)
            {
                _heightNoiseFilter.BlendMode = (BlendMode)CustomEditorGUI.EnumPopup(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Blend Mode"), _heightNoiseFilter.BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            CreateNoiseSettingsIfNecessary();

            DrawHeightSettings(ref rect);

            noiseSettingsGUI.OnGUI(rect);
        }

        private void DrawHeightSettings(ref Rect rect)
        {
            _heightNoiseFilter.MinHeight = CustomEditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Height"),
                _heightNoiseFilter.MinHeight);

            rect.y += EditorGUIUtility.singleLineHeight;

            _heightNoiseFilter.MaxHeight = CustomEditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Height"),
                _heightNoiseFilter.MaxHeight);

            rect.y += EditorGUIUtility.singleLineHeight;

            _heightNoiseFilter.MinRangeNoise = CustomEditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Min Range Noise"), _heightNoiseFilter.MinRangeNoise);
            rect.y += EditorGUIUtility.singleLineHeight;
            _heightNoiseFilter.MaxRangeNoise = CustomEditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Max Range Noise"), _heightNoiseFilter.MaxRangeNoise);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            CreateNoiseSettingsIfNecessary();

            float height = 0;

            if (index != 0)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight * 4;

            if (_heightNoiseFilter.NoiseSettings.ShowNoisePreviewTexture)
            {
                height += 256f + EditorGUIUtility.singleLineHeight * 2;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;

            if (_heightNoiseFilter.NoiseSettings.ShowNoiseTransformSettings)
            {
                height += EditorGUIUtility.singleLineHeight * 7;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;

            if (_heightNoiseFilter.NoiseSettings.ShowNoiseTypeSettings)
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
            if (_heightNoiseFilter.NoiseSettings == null)
            {
                _heightNoiseFilter.NoiseSettings = new NoiseSettings();
            }
        }
    }
}
#endif
