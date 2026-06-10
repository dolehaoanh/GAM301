#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.DeepCopy.Runtime;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.Popup.Editor.Selector;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.ReflectionUtility.Runtime;
using ReorderableList = VladislavTsurikov.UIToolkitReorderableList.Runtime.ReorderableList;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.UIToolkitReorderableList
{
    public class ReorderableListStackEditor<T, N> : NodeStackEditor<T, N>
        where T : Node
        where N : UIToolkitReorderableListComponentEditor
    {
        private readonly AdvancedNodeStack<T> _advancedNodeStack;
        private readonly GUIContent _listName;
        private Node _copyComponentElement;

        private ReorderableList _reorderableList;

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
            _advancedNodeStack = stack;
            _listName = new GUIContent("");
            InitializeReorderableList();
        }

        public ReorderableListStackEditor(GUIContent listName, AdvancedNodeStack<T> stack,
            bool displayHeader) : base(stack)
        {
            _listName = listName;
            InitializeReorderableList();
        }

        private void InitializeReorderableList()
        {
            _reorderableList = new ReorderableList {
                HeaderTitle = _listName?.text ?? "Components",
                ShowHeader = DisplayHeaderText,
                ShowFooter = DisplayPlusButton,
                ShowAddButton = DisplayPlusButton,
                ShowRemoveButton = true,
                Reorderable = ReorderSupport
            };

            _reorderableList.SetItemType(typeof(T));
            _reorderableList.MakeItem = MakeListItem;
            _reorderableList.BindItem = BindListItem;
            _reorderableList.OnAdd = OnAdd;
            _reorderableList.OnRemove = OnRemove;
            _reorderableList.OnReorder = OnReorder;

            RefreshList();
        }

        public VisualElement GetVisualElement()
        {
            if (Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();
                RefreshEditors();
                RefreshList();
                Stack.IsDirty = false;
            }

            return _reorderableList;
        }

        private VisualElement MakeListItem()
        {
            ComponentListElement element = new();

            element.OnMenuClicked = HandleMenuClick;
            element.OnSelectionChanged = HandleSelection;
            element.OnActiveChanged = HandleActiveChanged;
            element.OnRenameComplete = HandleRenameComplete;
            element.SetDragHandleVisible(ReorderSupport);

            return element;
        }

        private void BindListItem(VisualElement visualElement, int index)
        {
            if (visualElement is not ComponentListElement componentElement)
            {
                return;
            }

            if (index < 0 || index >= Stack.ElementList.Count || index >= Editors.Count)
            {
                return;
            }

            T component = Stack.ElementList[index];
            N editor = Editors[index];

            if (component == null || editor == null)
            {
                return;
            }

            componentElement.SetComponent(component, ShowActiveToggle);
            componentElement.IsSelected = _reorderableList.SelectedIndex == index;

            // Create content for foldout
            VisualElement content = editor.CreateGUI();
            componentElement.SetContent(content);
        }

        private void HandleMenuClick(ComponentListElement element)
        {
            int index = GetElementIndex(element);
            if (index >= 0 && index < Stack.ElementList.Count)
            {
                ShowContextMenu(Stack.ElementList[index], index);
            }
        }

        private void HandleSelection(ComponentListElement element)
        {
            int index = GetElementIndex(element);
            if (index >= 0)
            {
                _reorderableList.SelectedIndex = index;
                Stack.Select(Stack.ElementList[index]);
                _reorderableList.Refresh();
            }
        }

        private void HandleActiveChanged(ComponentListElement element, bool active)
        {
            int index = GetElementIndex(element);
            if (index >= 0 && index < Stack.ElementList.Count)
            {
                Stack.ElementList[index].Active = active;
            }
        }

        private void HandleRenameComplete(ComponentListElement element, string newName)
        {
            int index = GetElementIndex(element);
            if (index >= 0 && index < Stack.ElementList.Count)
            {
                Stack.ElementList[index].Name = newName;
                Stack.ElementList[index].Renaming = false;
                _reorderableList.Refresh();
            }
        }

        private int GetElementIndex(ComponentListElement element)
        {
            VisualElement parent = element.parent;
            if (parent == null)
            {
                return -1;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent[i] == element)
                {
                    return i;
                }
            }

            return -1;
        }

        private void OnAdd(IList list)
        {
            ShowAddMenu();
        }

        private void OnRemove(int index)
        {
            if (index >= 0 && index < Stack.ElementList.Count)
            {
                T component = Stack.ElementList[index];
                if (component.IsDeletable())
                {
                    Stack.Remove(index);
                    RefreshEditors();
                    RefreshList();
                }
            }
        }

        private void OnReorder()
        {
            Stack.IsDirty = true;
        }

        private void RefreshList()
        {
            _reorderableList.ItemsSource = _advancedNodeStack.List;
            _reorderableList.Refresh();
        }

        protected virtual void ShowAddMenu()
        {
            Rect rect = new(Event.current != null ? Event.current.mousePosition : Vector2.zero,
                new Vector2(260f, 1f));
            ShowAddPopup(rect);
        }

        protected void ShowAddPopup(Rect buttonRect)
        {
            List<UIToolkitAddPopupItem> items = new();
            foreach (Type componentType in GetComponentTypes())
            {
                if (componentType.GetAttribute(typeof(DontCreateAttribute)) != null)
                {
                    continue;
                }

                if (componentType.GetAttribute<PersistentAttribute>() != null ||
                    componentType.GetAttribute<DontShowInAddMenuAttribute>() != null)
                {
                    continue;
                }

                UIToolkitAddPopupItem item = BuildAddItem(componentType);
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
                i =>
                {
                    i.OnPick?.Invoke();
                    RefreshEditors();
                    RefreshList();
                },
                i => i.IsEnabled);
        }

        /// <summary>
        ///     Returns popup metadata for a candidate type, or null to skip.
        ///     Override to customize path/enabled/creation per type.
        /// </summary>
        protected virtual UIToolkitAddPopupItem BuildAddItem(Type componentType)
        {
            string name = componentType.GetAttribute<NameAttribute>()?.Name ?? componentType.Name;

            if (Stack is NodeStackSupportSameType<T> sameTypes)
            {
                return new UIToolkitAddPopupItem {
                    Type = componentType, Path = name, IsEnabled = true,
                    OnPick = () => sameTypes.CreateNode(componentType)
                };
            }

            if (Stack is NodeStackOnlyDifferentTypes<T> onlyDifferent)
            {
                return new UIToolkitAddPopupItem {
                    Type = componentType, Path = name, IsEnabled = !onlyDifferent.HasType(componentType),
                    OnPick = () => onlyDifferent.CreateIfMissingType(componentType)
                };
            }

            return null;
        }

        protected virtual void ShowContextMenu(T component, int index)
        {
            GenericMenu menu = new();

            menu.AddItem(new GUIContent("Reset"), false, () =>
            {
                Stack.Reset(index);
                RefreshEditors();
                RefreshList();
            });

            if (RemoveSupport)
            {
                bool canRemove = component.IsDeletable() &&
                                 (Stack.AllowRemoveLastElement || Stack.ElementList.Count > 1);

                Debug.Log("canRemove " + canRemove);
                if (canRemove)
                {
                    menu.AddItem(new GUIContent("Remove"), false, () =>
                    {
                        Stack.Remove(index);
                        RefreshEditors();
                        RefreshList();
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Remove"));
                }
            }

            if (DuplicateSupport)
            {
                menu.AddItem(new GUIContent("Duplicate"), false, () =>
                {
                    DuplicateComponent(component, index + 1);
                    RefreshEditors();
                    RefreshList();
                });
            }

            if (RenameSupport)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Rename"), component.Renaming, () => { RenameComponent(component); });
            }

            if (CopySettings)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Copy Settings"), false,
                    () => _copyComponentElement = DeepCopier.Copy(component));

                if (_copyComponentElement != null)
                {
                    menu.AddItem(new GUIContent("Paste Settings"), false,
                        () =>
                        {
                            Stack.ReplaceElement((T)DeepCopier.Copy(_copyComponentElement), index);
                            RefreshEditors();
                            RefreshList();
                        });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste Settings"), false);
                }
            }

            menu.ShowAsContext();
        }

        private void DuplicateComponent(T component, int index)
        {
            if (Stack is NodeStackSupportSameType<T> componentStackWithSameTypes)
            {
                componentStackWithSameTypes.CreateNode(component.GetType(), index);
                Stack.ReplaceElement(DeepCopier.Copy(component), index);
            }
        }

        private void RenameComponent(Node component)
        {
            component.Renaming = !component.Renaming;
            component.RenamingName = component.Name;

            // Find the visual element for this component and start rename
            ListView listView = _reorderableList.Q<ListView>();
            if (listView != null)
            {
                int index = Stack.IndexOf((T)component);
                if (index >= 0)
                {
                    _reorderableList.Refresh();

                    // Delay to ensure the element is rendered
                    _reorderableList.schedule.Execute(() =>
                    {
                        ScrollView scrollView = listView.Q<ScrollView>();
                        if (scrollView?.contentContainer != null && index < scrollView.contentContainer.childCount)
                        {
                            VisualElement itemElement = scrollView.contentContainer[index];
                            ComponentListElement componentElement = itemElement.Q<ComponentListElement>();
                            componentElement?.StartRename();
                        }
                    }).ExecuteLater(50);
                }
            }
        }

        protected List<Type> GetComponentTypes()
        {
            return AllTypesDerivedFrom<T>.Types.OrderBy(x => x.FullName).ThenBy(x => x.Namespace?.Split('.')[^1])
                .ToList();
        }
    }
}
#endif
