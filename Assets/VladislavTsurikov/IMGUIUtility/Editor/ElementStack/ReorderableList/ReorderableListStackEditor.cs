#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.DeepCopy.Runtime;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.Popup.Editor.Selector;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList
{
    public class ReorderableListStackEditor<T, N> : NodeStackEditor<T, N>
        where T : Node
        where N : ReorderableListComponentEditor
    {
        private static class Styles
        {
            public static readonly Texture2D Move;

            static Styles() => Move = Resources.Load<Texture2D>("move");
        }

        private static Node _copyNodeElement;

        private readonly UnityEditorInternal.ReorderableList _reorderableList;
        private readonly GUIContent _reorderableListName;

        private bool _displayAddButton;
        private bool _dragging;
        protected bool CopySettings = true;

        public bool DisplayHeaderText = true;
        public bool DisplayPlusButton = true;
        public bool DuplicateSupport = true;
        public bool RemoveSupport = true;
        public bool RenameSupport = false;
        public bool ReorderSupport = true;
        public bool ShowActiveToggle = true;

        public ReorderableListStackEditor(AdvancedNodeStack<T> stack) : base(stack)
        {
            _reorderableListName = new GUIContent("");
            _reorderableList =
                new UnityEditorInternal.ReorderableList(stack.List, typeof(T), true, true, false,
                    false);

            SetupCallbacks();
        }

        public ReorderableListStackEditor(GUIContent reorderableListName, AdvancedNodeStack<T> stack,
            bool displayHeader) : base(stack)
        {
            _reorderableListName = reorderableListName;
            _reorderableList = new UnityEditorInternal.ReorderableList(stack.List, typeof(T), true,
                displayHeader, false, false);

            SetupCallbacks();
        }

        protected void ShowAddPopup(Rect buttonRect)
        {
            List<AddPopupItem> items = new();
            foreach (Type settingsType in GetComponentTypes())
            {
                if (settingsType.GetAttribute(typeof(DontCreateAttribute)) != null)
                {
                    continue;
                }

                if (settingsType.GetAttribute<PersistentAttribute>() != null ||
                    settingsType.GetAttribute<DontShowInAddMenuAttribute>() != null)
                {
                    continue;
                }

                if (!Stack.AllowCreate(settingsType))
                {
                    continue;
                }

                AddPopupItem item = BuildAddItem(settingsType);
                if (item != null)
                {
                    items.Add(item);
                }
            }

            AddPopup.Open(
                buttonRect,
                "Add Component",
                items,
                i => i.Path,
                i => i.OnPick?.Invoke(),
                i => i.IsEnabled);
        }

        /// <summary>
        ///     Returns popup metadata for a candidate type, or null to skip it.
        ///     Override to customize path/enabled/creation per type.
        /// </summary>
        protected virtual AddPopupItem BuildAddItem(Type settingsType)
        {
            string name = settingsType.GetAttribute<NameAttribute>()?.Name;
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (Stack is NodeStackSupportSameType<T> sameTypes)
            {
                return new AddPopupItem {
                    Type = settingsType, Path = name, IsEnabled = true,
                    OnPick = () => sameTypes.CreateNode(settingsType)
                };
            }

            if (Stack is NodeStackOnlyDifferentTypes<T> onlyDifferent)
            {
                return new AddPopupItem {
                    Type = settingsType, Path = name, IsEnabled = !onlyDifferent.HasType(settingsType),
                    OnPick = () => onlyDifferent.CreateIfMissingType(settingsType)
                };
            }

            return null;
        }

        protected virtual void AddCB(UnityEditorInternal.ReorderableList list)
        {
            Rect rect = new(Event.current.mousePosition, new Vector2(240f, 1f));
            ShowAddPopup(rect);
        }

        protected virtual void DrawHeaderElement(Rect totalRect, int index, N componentEditor)
        {
            Rect headerRect = totalRect;

            headerRect.x += 15;
            headerRect.height = EditorGUIUtility.singleLineHeight * 1.3f;

            if (ShowActiveToggle && componentEditor.Target.ShowActiveToggle())
            {
                bool temporaryActive = ((Node)componentEditor.Target).Active;

                componentEditor.Target.SelectSettingsFoldout = CustomEditorGUI.HeaderWithMenu(headerRect,
                    componentEditor.Target.Name,
                    componentEditor.Target.SelectSettingsFoldout, ref temporaryActive,
                    () => Menu(Stack.ElementList[index], index));

                ((Node)componentEditor.Target).Active = temporaryActive;
            }
            else
            {
                componentEditor.Target.SelectSettingsFoldout = CustomEditorGUI.HeaderWithMenu(headerRect,
                    componentEditor.Target.Name,
                    componentEditor.Target.SelectSettingsFoldout, () => Menu(Stack.ElementList[index], index));
            }
        }

        protected virtual void DrawElement(Rect totalRect, int index, float iconSize, Color prevColor,
            N componentEditor)
        {
            using (new EditorGUI.DisabledScope(!Stack.ElementList[index].Active))
            {
                float rectX;

                if (ReorderSupport)
                {
                    rectX = 35f;
                }
                else
                {
                    rectX = 35f - iconSize;
                }

                totalRect.x += rectX;
                totalRect.y += EditorGUIUtility.singleLineHeight + 3;
                totalRect.width -= rectX + 15;
                totalRect.height = EditorGUIUtility.singleLineHeight;

                GUI.color = prevColor;

                if (componentEditor.Target.SelectSettingsFoldout)
                {
                    try
                    {
                        componentEditor.OnGUI(totalRect, index);
                    }
                    catch (Exception ex) when (ex is not ExitGUIException)
                    {
                        Debug.LogError("ComponentEditor has an error: " + ex.Message);
                        Debug.LogError("Stack trace: " + ex.StackTrace);
                    }
                }
            }
        }

        protected virtual void Menu(T node, int index)
        {
            GenericMenu menu = new();
            menu.AddItem(new GUIContent("Reset"), false, () => Stack.Reset(index));

            if (RemoveSupport)
            {
                bool canRemove = node.IsDeletable() &&
                                 (Stack.AllowRemoveLastElement || Stack.ElementList.Count > 1);
                if (canRemove)
                {
                    menu.AddItem(new GUIContent("Remove"), false, () => Stack.Remove(index));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Remove"));
                }
            }

            if (DuplicateSupport)
            {
                menu.AddItem(new GUIContent("Duplicate"), false, () => DuplicateComponent(node, index + 1));
            }

            if (RenameSupport)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Rename"), node.Renaming, () => RenameComponent(node));
            }

            if (CopySettings)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Copy Settings"), false,
                    () => _copyNodeElement = DeepCopier.Copy(node));

                bool canPaste = _copyNodeElement != null &&
                                node.GetType().IsAssignableFrom(_copyNodeElement.GetType());

                if (canPaste)
                {
                    menu.AddItem(new GUIContent("Paste Settings"), false,
                        () => Stack.ReplaceElement((T)DeepCopier.Copy(_copyNodeElement), index));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste Settings"), false);
                }
            }

            ExtendMenu(menu, node, index);
            menu.ShowAsContext();
        }

        protected virtual void ExtendMenu(GenericMenu menu, T node, int index)
        {
            DocUrlAttribute docAttr = node.GetType().GetAttribute<DocUrlAttribute>();
            if (docAttr != null)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Documentation"), false,
                    () => Application.OpenURL(docAttr.Url));
            }
        }

        private void DuplicateComponent(T component, int index)
        {
            if (Stack is NodeStackSupportSameType<T> componentStackWithSameTypes)
            {
                componentStackWithSameTypes.CreateNode(component.GetType(), index);
                Stack.ReplaceElement(DeepCopier.Copy(component), index);
            }
        }

        private void SetupCallbacks()
        {
            if (_reorderableListName != null)
            {
                _reorderableList.drawHeaderCallback = DrawHeaderCB;
            }

            _reorderableList.drawElementCallback = DrawElementCB;
            _reorderableList.elementHeightCallback = ElementHeightCB;
            _reorderableList.onAddCallback = AddCB;
            _reorderableList.onRemoveCallback = RemoveElement;
            _reorderableList.onChangedCallback = OnChangedCallback;
        }

        private void OnChangedCallback(UnityEditorInternal.ReorderableList list)
        {
            Stack.IsDirty = true;
        }

        public virtual void OnGUI()
        {
            if (Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();
                RefreshEditors();
                Stack.IsDirty = false;
            }

            _reorderableList.draggable = ReorderSupport;
            Rect rect = EditorGUILayout.GetControlRect(true, _reorderableList.GetHeight());
            rect = EditorGUI.IndentedRect(rect);

            _reorderableList.DoList(rect);
        }

        public void OnGUI(Rect rect)
        {
            if (Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();
                RefreshEditors();
                Stack.IsDirty = false;
            }

            _reorderableList.draggable = ReorderSupport;
            OnReorderableListStackGUI(rect);
        }

        protected virtual void OnReorderableListStackGUI(Rect rect)
        {
            _reorderableList.DoList(rect);
        }

        public virtual float GetElementStackHeight()
        {
            float height = CustomEditorGUI.SingleLineHeight * 3;

            for (int i = 0; i < Editors.Count; i++)
            {
                height += CustomEditorGUI.SingleLineHeight;

                if (Editors[i].Target.Renaming)
                {
                    continue;
                }

                if (Editors[i].Target.SelectSettingsFoldout)
                {
                    height += Editors[i].GetElementHeight(i);
                }
            }

            return height;
        }

        private void RemoveElement(UnityEditorInternal.ReorderableList list)
        {
            Stack.Remove(list.index);
        }

        private void DrawHeaderCB(Rect rect)
        {
            if (DisplayHeaderText)
            {
                CustomEditorGUI.Label(rect, _reorderableListName.text,
                    CustomEditorGUI.GetStyle(StyleName.LabelFoldout));
            }

            if (DisplayPlusButton)
            {
                DrawPlusButton(rect);
            }
        }

        private void DrawPlusButton(Rect rect)
        {
            Rect buttonRect = rect;
            buttonRect.x += rect.width - EditorGUIUtility.singleLineHeight - 3;

            Color color = GUI.color;
            Rect menuRect = new(buttonRect.x, buttonRect.y + 2f, 14, 14);

            if (CustomEditorGUI.DrawIcon(menuRect, StyleName.IconButtonPlus, EditorColors.Instance.Green))
            {
                ShowAddPopup(menuRect);
                Event.current.Use();
            }

            GUI.color = color;
        }

        public virtual float ElementHeightCB(int index)
        {
            if (Editors.Count == 0)
            {
                return 0;
            }

            try
            {
                N editor = Editors[index];
            }
            catch (Exception)
            {
                Debug.Log(Editors.Count);
                Debug.Log("index " + index);
                throw;
            }

            N componentEditor = Editors[index];

            float height = EditorGUIUtility.singleLineHeight * 1.5f;

            if (componentEditor == null)
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }

            if (!componentEditor.Target.SelectSettingsFoldout)
            {
                return EditorGUIUtility.singleLineHeight + 5;
            }

            if (RenameSupport)
            {
                if (componentEditor.Target.Renaming)
                {
                    return EditorGUIUtility.singleLineHeight + 5;
                }

                height += componentEditor.GetElementHeight(index);
                return height;
            }

            height += componentEditor.GetElementHeight(index);
            return height;
        }

        private void DrawElementCB(Rect totalRect, int index, bool isActive, bool isFocused)
        {
            if (Editors.Count == 0)
            {
                return;
            }

            if (Stack.IsDirty)
            {
                RefreshEditors();
            }

            float dividerSize = 1f;
            float paddingV = 6f;
            float paddingH = 4f;
            float iconSize = 14f;

            bool isSelected = _reorderableList.index == index;

            Color bgColor;

            if (EditorGUIUtility.isProSkin)
            {
                if (isSelected)
                {
                    ColorUtility.TryParseHtmlString("#424242", out bgColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#383838", out bgColor);
                }
            }
            else
            {
                if (isSelected)
                {
                    ColorUtility.TryParseHtmlString("#b4b4b4", out bgColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#c2c2c2", out bgColor);
                }
            }

            Color dividerColor;

            if (isSelected)
            {
                dividerColor = EditorColors.Instance.ToggleButtonActiveColor;
            }
            else
            {
                if (EditorGUIUtility.isProSkin)
                {
                    ColorUtility.TryParseHtmlString("#202020", out dividerColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#a8a8a8", out dividerColor);
                }
            }

            Color prevColor = GUI.color;

            if (ReorderSupport)
            {
                totalRect.xMin -= 20f;
                totalRect.xMax += 4f;
            }
            else
            {
                totalRect.xMin -= 6f;
                totalRect.xMax += 3f;
            }

            bool containsMouse = totalRect.Contains(Event.current.mousePosition);

            if (containsMouse && Event.current.type == EventType.MouseDown)
            {
                _reorderableList.index = index;
            }

            Rect separatorRect = totalRect;
            GUI.color = dividerColor;
            GUI.DrawTexture(separatorRect, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.color = prevColor;

            totalRect.yMin += dividerSize;
            totalRect.xMin += dividerSize;
            totalRect.xMax -= dividerSize;
            totalRect.yMax -= dividerSize;

            GUI.color = bgColor;
            GUI.DrawTexture(totalRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false);

            GUI.color = new Color(.7f, .7f, .7f, 1f);

            N componentEditor = Editors[index];

            if (componentEditor == null)
            {
                return;
            }

            if (componentEditor.Target.Renaming)
            {
                GUI.color = new Color(1f, 1f, 1f, 1f);

                const float plusRectX = 5;

                totalRect.x += plusRectX;
                totalRect.y += 2;
                totalRect.width -= plusRectX + 15;
                totalRect.height = EditorGUIUtility.singleLineHeight;

                RenameComponentGUI(totalRect, componentEditor.Target);

                GUI.color = prevColor;
                return;
            }

            if (ReorderSupport)
            {
                Rect moveRect = new(totalRect.xMin + paddingH, totalRect.yMin + paddingV, iconSize, iconSize);
                EditorGUIUtility.AddCursorRect(moveRect, MouseCursor.Pan);
                GUI.DrawTexture(moveRect, Styles.Move, ScaleMode.StretchToFill);
            }

            GUI.color = new Color(1f, 1f, 1f, 1f);

            DrawHeaderElement(totalRect, index, componentEditor);

            if (containsMouse && isSelected)
            {
                if (Event.current.type == EventType.MouseDrag && !_dragging && isFocused)
                {
                    _dragging = true;
                    _reorderableList.index = index;
                }
            }

            if (_dragging)
            {
                if (Event.current.type == EventType.MouseUp)
                {
                    _dragging = false;
                }
            }

            DrawElement(totalRect, index, iconSize, prevColor, componentEditor);

            GUI.color = prevColor;
        }


        private void RenameComponent(Node node)
        {
            node.Renaming = !node.Renaming;
            node.RenamingName = node.Name;
        }

        private void RenameComponentGUI(Rect rect, Element stackElement)
        {
            if (!stackElement.Renaming)
            {
                return;
            }

            Color initialGUIColor = GUI.color;

            Rect rectField = CustomEditorGUI.PrefixLabel(rect, new GUIContent("Rename to"));

            rectField.x -= 28;

            GUI.color = EditorColors.Instance.orangeNormal;
            stackElement.RenamingName = EditorGUI.TextField(rectField, GUIContent.none, stackElement.RenamingName);
            GUI.color = initialGUIColor;

            Rect iconRect = new(rectField.x + rectField.width + 2, rectField.y + 1, 14, 14);

            if (CustomEditorGUI.DrawIcon(iconRect, StyleName.IconButtonOk, EditorColors.Instance.Green) ||
                (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyUp)) //rename OK button
            {
                stackElement.Renaming = false;
                stackElement.Name = stackElement.RenamingName;

                Event.current.Use();
            }

            iconRect.x += 16;

            if (CustomEditorGUI.DrawIcon(iconRect, StyleName.IconButtonCancel,
                    EditorColors.Instance.Red)) //rename CANCEL button
            {
                stackElement.RenamingName = stackElement.Name;
                stackElement.Renaming = false;

                Event.current.Use();
            }

            GUI.color = initialGUIColor;

            GUILayout.Space(15);
        }

        protected List<Type> GetComponentTypes()
        {
            return AllTypesDerivedFrom<T>.Types.OrderBy(x => x.FullName).ThenBy(x => x.Namespace?.Split('.')[^1])
                .ToList();
        }
    }
}
#endif
