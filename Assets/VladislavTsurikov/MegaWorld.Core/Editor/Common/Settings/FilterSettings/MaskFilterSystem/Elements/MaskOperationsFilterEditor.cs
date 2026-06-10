#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [ElementEditor(typeof(MaskOperationsFilter))]
    public class MaskOperationsFilterEditor : ReorderableListComponentEditor
    {
        private MaskOperationsFilter _maskOperationsFilter;

        public override void OnEnable()
        {
            _maskOperationsFilter = (MaskOperationsFilter)Target;
        }

        public override void OnGUI(Rect rect, int index)
        {
            _maskOperationsFilter.MaskOperations = (MaskOperations)CustomEditorGUI.EnumPopup(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                _maskOperationsFilter.MaskOperations);

            rect.y += EditorGUIUtility.singleLineHeight;

            switch (_maskOperationsFilter.MaskOperations)
            {
                case MaskOperations.Add:
                {
                    _maskOperationsFilter.Value =
                        CustomEditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            new GUIContent("Value"), _maskOperationsFilter.Value, 0f, 1f);
                    break;
                }
                case MaskOperations.Multiply:
                {
                    _maskOperationsFilter.Value =
                        CustomEditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            new GUIContent("Value"), _maskOperationsFilter.Value, 0f, 1f);
                    break;
                }
                case MaskOperations.Power:
                {
                    _maskOperationsFilter.Value =
                        CustomEditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            new GUIContent("Value"), _maskOperationsFilter.Value, 1f, 10f);
                    break;
                }
                case MaskOperations.Clamp:
                {
                    CustomEditorGUI.MinMaxSlider(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Range"), ref _maskOperationsFilter.ClampRange.x,
                        ref _maskOperationsFilter.ClampRange.y, 0, 1);

                    rect.y += EditorGUIUtility.singleLineHeight;
                    break;
                }
                case MaskOperations.Invert:
                {
                    _maskOperationsFilter.StrengthInvert = CustomEditorGUI.Slider(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Strength"), _maskOperationsFilter.StrengthInvert, 0f, 1f);

                    break;
                }
                case MaskOperations.Remap:
                {
                    CustomEditorGUI.MinMaxSlider(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Range"), ref _maskOperationsFilter.RemapRange.x,
                        ref _maskOperationsFilter.RemapRange.y, 0, 1);

                    rect.y += EditorGUIUtility.singleLineHeight;
                    break;
                }
            }
        }

        public override float GetElementHeight(int index)
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            switch (_maskOperationsFilter.MaskOperations)
            {
                case MaskOperations.Add:
                case MaskOperations.Multiply:
                case MaskOperations.Power:
                case MaskOperations.Invert:
                {
                    height += EditorGUIUtility.singleLineHeight;
                    break;
                }
                case MaskOperations.Clamp:
                case MaskOperations.Remap:
                {
                    height += EditorGUIUtility.singleLineHeight;
                    break;
                }
            }

            return height;
        }
    }
}
#endif
