using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UIToolkitUtility.Editor.ElementStack.TabStack
{
    public class TabStackEditor<T> : VisualElement
    {
        public delegate void AddCallbackDelegate();

        public delegate bool IsSelectedCallbackDelegate(T item);

        public delegate void MoveCallbackDelegate();

        public delegate void RenameCallbackDelegate(int index, string newName);

        public delegate void SelectCallbackDelegate(int index);

        public delegate void TabColorCallbackDelegate(T item, out Color barColor, out Color labelColor);

        public delegate GenericMenu TabMenuCallbackDelegate(int index);

        public delegate string TabNameCallbackDelegate(T item);

        private const float DragIndicatorWidth = 2f;

        private const string LayoutPath =
            "Assets/VladislavTsurikov/UIToolkitUtility/Editor/ElementStack/TabStack/TabStackEditor.uxml";

        private const string StylePath =
            "Assets/VladislavTsurikov/UIToolkitUtility/Editor/ElementStack/TabStack/TabStackEditor.uss";

        private const string LayoutContainerName = "LayoutContainer";
        private const string DragIndicatorName = "DragIndicator";

        private readonly IList<T> _elements;
        private readonly List<Tab> _tabButtons = new();
        private bool _dragAfter;
        private int _dragIndex = -1;
        private VisualElement _dragIndicator;
        private int _dragTargetIndex = -1;
        private VisualElement _root;

        public AddCallbackDelegate AddCallback;
        public bool Draggable = true;
        public bool EnableRename;
        public IsSelectedCallbackDelegate IsSelected;
        public MoveCallbackDelegate MoveCallback;

        public int OffsetTabWidth = 30;
        public RenameCallbackDelegate RenameCallback;
        public SelectCallbackDelegate SelectCallback;
        public Color SelectedBackgroundColor = new(0.4f, 0.6f, 0.8f);
        public TabColorCallbackDelegate TabColor;
        public int TabHeight = 25;
        public TabMenuCallbackDelegate TabMenuCallback;
        public TabNameCallbackDelegate TabName;
        public int TabPlusWidth = 50;
        public int TabWidth = 130;
        public bool TabWidthFromName = true;

        public TabStackEditor(IList<T> elements)
        {
            _elements = elements;
            CreateRoot();

            _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            _root.RegisterCallback<PointerUpEvent>(OnPointerUp);

            Refresh();
        }

        public VisualElement CreateGUI()
        {
            return this;
        }

        private void CreateRoot()
        {
            VisualTreeAsset layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LayoutPath);
            if (layout == null)
            {
                _root = this;
                _root.style.flexDirection = FlexDirection.Row;
                _root.style.flexWrap = Wrap.Wrap;

                _dragIndicator = CreateFallbackDragIndicator();
                _root.Add(_dragIndicator);
                return;
            }

            TemplateContainer template = layout.CloneTree();
            Add(template);
            _root = template.Q<VisualElement>(LayoutContainerName) ?? template;

            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath);
            if (styleSheet != null)
            {
                styleSheets.Add(styleSheet);
            }

            _dragIndicator = template.Q<VisualElement>(DragIndicatorName) ?? CreateFallbackDragIndicator();
            if (_dragIndicator.parent == null)
            {
                _root.Add(_dragIndicator);
            }
        }

        private static VisualElement CreateFallbackDragIndicator()
        {
            VisualElement dragIndicator = new() { name = DragIndicatorName };
            dragIndicator.style.position = Position.Absolute;
            dragIndicator.style.width = DragIndicatorWidth;
            dragIndicator.style.backgroundColor = Color.white;
            dragIndicator.style.display = DisplayStyle.None;
            return dragIndicator;
        }

        public void Refresh()
        {
            if (_root == null)
            {
                return;
            }

            _root.Clear();
            _root.Add(_dragIndicator);
            _tabButtons.Clear();

            for (int index = 0; index < _elements.Count; index++)
            {
                Tab tabButton = CreateTabButton(index);
                _tabButtons.Add(tabButton);
                _root.Add(tabButton);
            }

            if (AddCallback != null)
            {
                TabPlus addButton = new();
                addButton.Clicked += () => AddCallback();
                ApplyTabSize(addButton, TabPlusWidth);
                _root.Add(addButton);
            }
        }

        private Tab CreateTabButton(int index)
        {
            T item = _elements[index];
            Tab tabButton = new();
            tabButton.Clicked += () => Select(index);
            tabButton.Text = TabName?.Invoke(item) ?? item?.ToString() ?? "Tab";

            ApplyTabSize(tabButton, GetTabWidth(tabButton.Text));

            if (TabColor != null)
            {
                TabColor(item, out Color barColor, out Color labelColor);
                tabButton.SetBackgroundColor(barColor);
                tabButton.SetLabelColor(labelColor);
            }
            else if (IsSelected != null && IsSelected(item))
            {
                tabButton.SetBackgroundColor(SelectedBackgroundColor);
            }

            tabButton.RegisterCallback<ContextClickEvent>(evt =>
            {
                evt.StopPropagation();
                if (TabMenuCallback == null)
                {
                    return;
                }

                if (IsSelected != null && !IsSelected(item))
                {
                    Select(index);
                    return;
                }

                GenericMenu menu = TabMenuCallback(index);
                if (EnableRename && RenameCallback != null)
                {
                    menu.AddItem(new GUIContent("Rename"), false, () => BeginRename(index));
                }

                menu.ShowAsContext();
            });

            tabButton.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (!Draggable || evt.button != 0)
                {
                    return;
                }

                _dragIndex = index;
                _dragTargetIndex = index;
                _dragAfter = false;
                tabButton.CapturePointer(evt.pointerId);
            });

            return tabButton;
        }

        private void ApplyTabSize(VisualElement tabButton, float width)
        {
            tabButton.style.height = TabHeight;
            tabButton.style.minHeight = TabHeight;
            tabButton.style.width = width;
            tabButton.style.minWidth = width;
        }

        private float GetTabWidth(string text)
        {
            if (!TabWidthFromName)
            {
                return TabWidth;
            }

            string safeText = string.IsNullOrEmpty(text) ? "Tab" : text;

            try
            {
                GUIStyle labelStyle = EditorStyles.label;
                if (labelStyle != null)
                {
                    return labelStyle.CalcSize(new GUIContent(safeText)).x + OffsetTabWidth;
                }
            }
            catch (NullReferenceException)
            {
            }

            // EditorStyles can be unavailable for a short time during domain reload.
            return Mathf.Max(50f, safeText.Length * 8f + OffsetTabWidth);
        }

        private void Select(int index)
        {
            SelectCallback?.Invoke(index);
        }

        private void BeginRename(int index)
        {
            if (_root == null || RenameCallback == null || index < 0 || index >= _tabButtons.Count)
            {
                return;
            }

            Tab tabButton = _tabButtons[index];
            TextField textField = new() { value = tabButton.Text };

            textField.style.width = tabButton.resolvedStyle.width;
            textField.style.height = tabButton.resolvedStyle.height;

            void ApplyRename(bool accept)
            {
                if (accept)
                {
                    RenameCallback(index, textField.value);
                }

                Refresh();
            }

            textField.RegisterCallback<BlurEvent>(_ => ApplyRename(true));
            textField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return)
                {
                    ApplyRename(true);
                }
                else if (evt.keyCode == KeyCode.Escape)
                {
                    ApplyRename(false);
                }
            });

            int buttonIndex = _root.IndexOf(tabButton);
            _root.Remove(tabButton);
            _root.Insert(buttonIndex, textField);
            textField.Focus();
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!Draggable || _dragIndex < 0)
            {
                return;
            }

            Vector3 pointerPosition = evt.position;
            for (int index = 0; index < _tabButtons.Count; index++)
            {
                Tab tab = _tabButtons[index];
                if (!tab.worldBound.Contains(pointerPosition))
                {
                    continue;
                }

                _dragTargetIndex = index;
                _dragAfter = pointerPosition.x > tab.worldBound.center.x;
                UpdateDragIndicator(tab);
                break;
            }
        }

        private void UpdateDragIndicator(VisualElement tab)
        {
            if (_dragIndicator == null)
            {
                return;
            }

            Rect bounds = tab.worldBound;
            float x = _dragAfter ? bounds.xMax : bounds.xMin;
            _dragIndicator.style.left = x - _root.worldBound.xMin;
            _dragIndicator.style.top = bounds.yMin - _root.worldBound.yMin;
            _dragIndicator.style.height = bounds.height;
            _dragIndicator.style.display = DisplayStyle.Flex;
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!Draggable || _dragIndex < 0)
            {
                return;
            }

            Move(_dragIndex, _dragTargetIndex, _dragAfter);
            _dragIndex = -1;
            _dragTargetIndex = -1;
            _dragAfter = false;
            _dragIndicator.style.display = DisplayStyle.None;
        }

        private void Move(int sourceIndex, int targetIndex, bool isAfter)
        {
            if (sourceIndex < 0 || targetIndex < 0 || sourceIndex == targetIndex || sourceIndex >= _elements.Count)
            {
                return;
            }

            int destinationIndex = targetIndex;
            if (sourceIndex > targetIndex)
            {
                if (isAfter)
                {
                    destinationIndex += 1;
                }
            }
            else
            {
                if (!isAfter)
                {
                    destinationIndex -= 1;
                }
            }

            destinationIndex = Mathf.Clamp(destinationIndex, 0, _elements.Count - 1);

            T item = _elements[sourceIndex];
            _elements.RemoveAt(sourceIndex);
            _elements.Insert(destinationIndex, item);

            MoveCallback?.Invoke();
            Refresh();
        }
    }
}
