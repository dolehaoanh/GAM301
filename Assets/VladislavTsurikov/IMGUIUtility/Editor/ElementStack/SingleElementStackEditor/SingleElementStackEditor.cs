#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Assemblies.VladislavTsurikov.Nody.Runtime.SingleElementStack;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.Popup.Editor.Selector;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.SingleElementStackEditor
{
    public class SingleElementStackEditor<T, N> : NodeStackEditor<T, N>
        where T : Node
        where N : IMGUIElementEditor
    {
        private readonly SingleElementStack<T> _singleElementStack;
        private GUIStyle _darkBackgroundStyle;

        public SingleElementStackEditor(SingleElementStack<T> stack) : base(stack) => _singleElementStack = stack;

        public void OnGUI()
        {
            if (Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();
                RefreshEditors();
                InitializeStyles();
                Stack.IsDirty = false;
            }

            Node node = _singleElementStack.GetElement();
            string clickButtonText = node == null ? "Select" : node.Name;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                if (CustomEditorGUILayout.ClickButton(clickButtonText))
                {
                    ShowAddPopup(GUILayoutUtility.GetLastRect());
                }

                GUILayout.Space(3);
            }
            GUILayout.EndHorizontal();

            if (node != null)
            {
                IMGUIElementEditor editor = GetElement();

                editor.OnGUI();
            }
        }

        private void ShowAddPopup(Rect buttonRect)
        {
            Node currentNode = _singleElementStack.GetElement();
            Type currentType = currentNode?.GetType();

            List<Type> types = new();
            foreach (Type settingsType in AllTypesDerivedFrom<T>.Types)
            {
                if (settingsType.GetAttribute<NameAttribute>() == null)
                {
                    continue;
                }

                types.Add(settingsType);
            }

            AddPopup.Open(
                buttonRect,
                "Select Type",
                types,
                t => t.GetAttribute<NameAttribute>().Name,
                selected => _singleElementStack.ReplaceElement(selected),
                t => t != currentType);
        }

        private void InitializeStyles()
        {
            if (_darkBackgroundStyle == null)
            {
                _darkBackgroundStyle = new GUIStyle(EditorStyles.helpBox);
                _darkBackgroundStyle.normal.background = MakeBackgroundTexture(1, 1, new Color(0.1f, 0.1f, 0.1f));
                _darkBackgroundStyle.padding = new RectOffset(10, 10, 10, 10);
            }
        }

        private IMGUIElementEditor GetElement()
        {
            return Editors.Count > 0 ? Editors[0] : null;
        }

        private Texture2D MakeBackgroundTexture(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }

            Texture2D result = new(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}
#endif
