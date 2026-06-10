#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack
{
    public class IMGUIComponentStackEditor<T, N> : NodeStackEditor<T, N>
        where T : Node
        where N : IMGUIElementEditor
    {
        public IMGUIComponentStackEditor(AdvancedNodeStack<T> stack) : base(stack)
        {
        }

        protected virtual void OnIMGUIComponentStackGUI()
        {
            for (int i = 0; i < Editors.Count; i++)
            {
                OnGUIElement(Editors[i], i);
            }
        }

        protected virtual void OnGUIElement(N editor, int index)
        {
            try
            {
                if (editor.GetType().GetAttribute<DontDrawFoldoutAttribute>() == null)
                {
                    editor.Target.SelectSettingsFoldout = CustomEditorGUILayout.HeaderWithMenu(editor.Target.Name,
                        editor.Target.SelectSettingsFoldout,
                        () => Menu(index)
                    );

                    if (editor.Target.SelectSettingsFoldout)
                    {
                        EditorGUI.indentLevel++;

                        editor.OnGUI();

                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    editor.OnGUI();
                }
            }
            catch (Exception ex) when (ex is not ExitGUIException)
            {
                Stack.Reset(index);
                throw;
            }
        }

        protected virtual void Menu(int index)
        {
            GenericMenu menu = new();
            menu.AddItem(new GUIContent("Reset"), false, () => Stack.Reset(index));

            DocUrlAttribute docAttr = Editors[index].Target.GetType().GetAttribute<DocUrlAttribute>();
            if (docAttr != null)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Documentation"), false,
                    () => Application.OpenURL(docAttr.Url));
            }

            menu.ShowAsContext();
        }

        public void OnGUI()
        {
            if (Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();
                RefreshEditors();
                Stack.IsDirty = false;
            }

            OnIMGUIComponentStackGUI();
        }

        public void DrawElements(List<Type> drawTypes)
        {
            foreach (Type type in drawTypes)
            {
                DrawElement(type);
            }
        }

        public void DrawElement(Type type)
        {
            if (Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();
                RefreshEditors();
                Stack.IsDirty = false;
            }

            for (int i = 0; i < Editors.Count; i++)
            {
                if (Editors[i].Target.GetType() == type)
                {
                    OnGUIElement(Editors[i], i);
                }
            }
        }

        public N GetEditor(Type type)
        {
            foreach (N editor in Editors)
            {
                if (editor.Target.GetType() == type)
                {
                    return editor;
                }
            }

            return null;
        }
    }
}
#endif
