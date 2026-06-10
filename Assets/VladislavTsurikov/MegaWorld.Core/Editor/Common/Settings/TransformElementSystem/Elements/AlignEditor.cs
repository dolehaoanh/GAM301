#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [Serializable]
    [ElementEditor(typeof(Align))]
    public class AlignEditor : ReorderableListComponentEditor
    {
        private Align _align;

        public override void OnEnable()
        {
            _align = (Align)Target;
        }

        public override void OnGUI(Rect rect, int index)
        {
            _align.UseNormalWeight =
                CustomEditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Use Normal Weight"), _align.UseNormalWeight);
            rect.y += EditorGUIUtility.singleLineHeight;

            if (_align.UseNormalWeight)
            {
                _align.MinMaxRange =
                    CustomEditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Min Max Range"), _align.MinMaxRange);
                rect.y += EditorGUIUtility.singleLineHeight;

                if (_align.MinMaxRange)
                {
                    CustomEditorGUI.MinMaxSlider(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Weight To Normal"), ref _align.MinWeightToNormal, ref _align.MaxWeightToNormal,
                        0, 1);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    _align.MaxWeightToNormal =
                        CustomEditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            new GUIContent("Weight To Normal"), _align.MaxWeightToNormal, 0, 1);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }
            }
        }

        public override float GetElementHeight(int index)
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            if (_align.UseNormalWeight)
            {
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
            }

            return height;
        }
    }
}
#endif
