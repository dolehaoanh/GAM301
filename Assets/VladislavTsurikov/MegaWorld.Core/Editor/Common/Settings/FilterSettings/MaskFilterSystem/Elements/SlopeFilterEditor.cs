#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [ElementEditor(typeof(SlopeFilter))]
    public class SlopeFilterEditor : ReorderableListComponentEditor
    {
        private SlopeFilter _slopeFilter;

        public override void OnEnable()
        {
            _slopeFilter = (SlopeFilter)Target;
        }

        public override void OnGUI(Rect rect, int index)
        {
            if (Terrain.activeTerrain == null)
            {
                CustomEditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "There is no terrain in the scene", MessageType.Warning);

                rect.y += EditorGUIUtility.singleLineHeight;
                return;
            }

            if (!Terrain.activeTerrain.drawInstanced)
            {
                CustomEditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Turn on Draw Instanced in all terrains, because this filter requires Normal Map",
                    MessageType.Warning);

                rect.y += EditorGUIUtility.singleLineHeight;
                return;
            }

            if (index != 0)
            {
                _slopeFilter.BlendMode = (BlendMode)CustomEditorGUI.EnumPopup(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Blend Mode"), _slopeFilter.BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            CustomEditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Slope"),
                ref _slopeFilter.MinSlope, ref _slopeFilter.MaxSlope, 0f, 90f);
            rect.y += EditorGUIUtility.singleLineHeight;

            _slopeFilter.SlopeFalloffType = (FalloffType)CustomEditorGUI.EnumPopup(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Slope Falloff Type"), _slopeFilter.SlopeFalloffType);

            rect.y += EditorGUIUtility.singleLineHeight;

            if (_slopeFilter.SlopeFalloffType != FalloffType.None)
            {
                _slopeFilter.SlopeFalloffMinMax = CustomEditorGUI.Toggle(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Slope Falloff Min Max"), _slopeFilter.SlopeFalloffMinMax);

                rect.y += EditorGUIUtility.singleLineHeight;

                if (_slopeFilter.SlopeFalloffMinMax)
                {
                    _slopeFilter.MinAddSlopeFalloff = Mathf.Max(0.1f,
                        CustomEditorGUI.FloatField(
                            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            new GUIContent("Min Add Slope Falloff"), _slopeFilter.MinAddSlopeFalloff));

                    rect.y += EditorGUIUtility.singleLineHeight;

                    _slopeFilter.MaxAddSlopeFalloff = Mathf.Max(0.1f,
                        CustomEditorGUI.FloatField(
                            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            new GUIContent("Max Add Slope Falloff"), _slopeFilter.MaxAddSlopeFalloff));
                }
                else
                {
                    _slopeFilter.AddSlopeFalloff = Mathf.Max(0.1f,
                        CustomEditorGUI.FloatField(
                            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            new GUIContent("Add Slope Falloff"), _slopeFilter.AddSlopeFalloff));
                }
            }
        }

        public override float GetElementHeight(int index)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (Terrain.activeTerrain == null)
            {
                height += EditorGUIUtility.singleLineHeight;
                return height;
            }

            if (!Terrain.activeTerrain.drawInstanced)
            {
                height += EditorGUIUtility.singleLineHeight;
                return height;
            }

            if (index != 0)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            if (_slopeFilter.SlopeFalloffType != FalloffType.None)
            {
                height += EditorGUIUtility.singleLineHeight;

                if (_slopeFilter.SlopeFalloffMinMax)
                {
                    height += EditorGUIUtility.singleLineHeight;
                    height += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
            }

            return height;
        }
    }
}
#endif
