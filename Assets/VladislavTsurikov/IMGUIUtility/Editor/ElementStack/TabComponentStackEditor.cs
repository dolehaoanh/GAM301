#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack
{
    public class TabComponentStackEditor<T, N> : NodeStackEditor<T, N>
        where T : Node
        where N : IMGUIElementEditor
    {
        protected readonly TabStackEditor _tabStackEditor;

        protected AdvancedNodeStack<T> AdvancedNodeStack => (AdvancedNodeStack<T>)Stack;

        public TabComponentStackEditor(AdvancedNodeStack<T> stack) : base(stack) =>
            _tabStackEditor = new TabStackEditor(stack.List, true, false) {
                AddCallback = ShowAddManu, AddTabMenuCallback = TabMenu, HappenedMoveCallback = HappenedMove,
                TabWidthFromName = true
            };

        public virtual void OnTabStackGUI()
        {
            _tabStackEditor.OnGUI();
        }

        public void DrawSelectedSettings()
        {
            if (Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();
                RefreshEditors();
                Stack.IsDirty = false;
            }

            if (Stack.ElementList.Count == 0)
            {
                return;
            }

            OnSelectedComponentGUI();
        }

        protected virtual void OnSelectedComponentGUI()
        {
            if (Stack.SelectedElement == null)
            {
                return;
            }

            SelectedEditor?.OnGUI();
        }

        protected virtual GenericMenu TabMenu(int currentTabIndex)
        {
            GenericMenu menu = new();

            if (Stack.ElementList.Count > 1)
            {
                menu.AddItem(new GUIContent("Delete"), false, ContextMenuUtility.ContextMenuCallback,
                    new Action(() => { Stack.Remove(currentTabIndex); }));
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Delete"));
            }

            DocUrlAttribute docAttr = Stack.ElementList[currentTabIndex].GetType().GetAttribute<DocUrlAttribute>();
            if (docAttr != null)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Documentation"), false,
                    () => Application.OpenURL(docAttr.Url));
            }

            return menu;
        }

        protected virtual void ShowAddManu()
        {
            GenericMenu menu = new();

            foreach (KeyValuePair<Type, Type> item in AllEditorTypes<T>.Types)
            {
                Type settingsType = item.Key;

                if (settingsType.GetAttribute(typeof(DontCreateAttribute)) != null)
                {
                    continue;
                }

                if (settingsType.GetAttribute<PersistentAttribute>() != null ||
                    settingsType.GetAttribute<DontShowInAddMenuAttribute>() != null)
                {
                    continue;
                }

                string context = settingsType.GetAttribute<NameAttribute>().Name;

                if (Stack is NodeStackSupportSameType<T> componentStackWithSameTypes)
                {
                    menu.AddItem(new GUIContent(context), false,
                        () => componentStackWithSameTypes.CreateNode(settingsType));
                }
                else if (Stack is NodeStackOnlyDifferentTypes<T> componentStackWithDifferentTypes)
                {
                    bool exists = componentStackWithDifferentTypes.HasType(settingsType);

                    if (!exists)
                    {
                        menu.AddItem(new GUIContent(context), false,
                            () => componentStackWithDifferentTypes.CreateIfMissingType(settingsType));
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent(context));
                    }
                }
            }

            menu.ShowAsContext();
        }

        private void HappenedMove()
        {
            Stack.IsDirty = true;
        }
    }
}
#endif
