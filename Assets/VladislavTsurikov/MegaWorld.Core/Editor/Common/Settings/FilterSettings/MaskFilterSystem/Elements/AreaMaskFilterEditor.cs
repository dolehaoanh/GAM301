#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [ElementEditor(typeof(AreaMaskFilter))]
    public class AreaMaskFilterEditor : ReorderableListComponentEditor
    {
        private AreaMaskFilter _areaMaskFilter;

        public override void OnEnable()
        {
            _areaMaskFilter = (AreaMaskFilter)Target;
        }

        public override void OnGUI(Rect rect, int index)
        {
            if (index != 0)
            {
                _areaMaskFilter.BlendMode = (BlendMode)CustomEditorGUI.EnumPopup(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Blend Mode"), _areaMaskFilter.BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            string[] ids = AreaMaskIdUtility.GetIds();
            if (ids.Length > 0)
            {
                string[] options = new string[ids.Length + 1];
                options[0] = "None";
                Array.Copy(ids, 0, options, 1, ids.Length);

                int selectedIndex = 0;
                for (int i = 0; i < ids.Length; i++)
                {
                    if (string.Equals(ids[i], _areaMaskFilter.MaskId, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedIndex = i + 1;
                        break;
                    }
                }

                selectedIndex = CustomEditorGUI.Popup(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Mask Id"), selectedIndex, options);
                _areaMaskFilter.MaskId = selectedIndex <= 0 ? string.Empty : options[selectedIndex];
                rect.y += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                _areaMaskFilter.MaskId = CustomEditorGUI.TextField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Mask Id"), _areaMaskFilter.MaskId);
                rect.y += EditorGUIUtility.singleLineHeight;

                CustomEditorGUI.HelpBox(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2f),
                    "No AreaMask IDs were found on active terrains. Synchronize a Gaea terrain first or type the expected ID manually.",
                    MessageType.Info);
                rect.y += EditorGUIUtility.singleLineHeight * 2f;
            }

            _areaMaskFilter.Orientation = (ImageMaskOrientation)CustomEditorGUI.EnumPopup(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Orientation"), _areaMaskFilter.Orientation);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            float height = EditorGUIUtility.singleLineHeight * 2f;
            if (index != 0)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            if (AreaMaskIdUtility.GetIds().Length == 0)
            {
                height += EditorGUIUtility.singleLineHeight * 2f;
            }

            return height;
        }
    }
}
#endif
