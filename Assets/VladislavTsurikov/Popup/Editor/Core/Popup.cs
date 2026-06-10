#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.Popup.Editor
{
    public abstract class Popup : EditorWindow
    {
        private const string PopupLayoutResourcePath = "Popup/Popup";
        private const string PopupStyleResourcePath = "Popup/PopupStyle";
        private const float DefaultMaxWindowHeight = 420f;
        private const float DefaultMinWindowHeight = 96f;
        private const float DefaultMinWindowWidth = 160f;
        private const float DefaultMaxWindowWidth = 260f;
        private const float ResizeHandleSize = 14f;

        private static readonly CustomStyleProperty<float> RowHeightProp = new("--popup-row-height");
        private static readonly CustomStyleProperty<float> SearchHeightProp = new("--popup-search-height");
        private static readonly CustomStyleProperty<float> ChromePaddingProp = new("--popup-chrome-padding");

        private Rect _activatorScreenRect;
        private Vector2 _actualSize;

        private bool _dragging;
        private Vector2 _dragStartMouse;
        private Vector2 _dragStartPosition;
        private Rect _mainWindowRect;
        private Vector2 _pendingMinSize = new(DefaultMinWindowWidth, DefaultMinWindowHeight);

        private string _pendingTitle;
        private Rect _pendingWindowRect;
        private VisualElement _resizeHandle;
        private Vector2 _resizeStartMouse;
        private Vector2 _resizeStartSize;

        private bool _resizing;

        private VisualElement _titleBar;
        private Label _titleLabel;

        protected VisualElement ContentHost { get; private set; }

        protected virtual bool ShowHeader => true;
        protected virtual bool CanDragWindow => true;
        protected virtual bool CanResizeWindow => true;
        protected virtual float MaxWindowHeight => DefaultMaxWindowHeight;
        protected virtual float MinWindowHeight => DefaultMinWindowHeight;
        protected virtual float MinWindowWidth => DefaultMinWindowWidth;
        protected virtual float MaxWindowWidth => DefaultMaxWindowWidth;

        protected float RowHeight { get; private set; } = 22f;

        protected float SearchHeight { get; private set; } = 21f;

        protected float ChromePadding { get; private set; } = 10f;

        internal Vector2 DefaultMinSize => new(MinWindowWidth, MinWindowHeight);
        internal Vector2 PreferredSize => GetPreferredWindowSize();

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualTreeAsset layout = Resources.Load<VisualTreeAsset>(PopupLayoutResourcePath);
            root.styleSheets.Add(Resources.Load<StyleSheet>(PopupStyleResourcePath));

            if (layout == null)
            {
                throw new InvalidOperationException(
                    $"Popup layout was not found at Resources/{PopupLayoutResourcePath}.");
            }

            layout.CloneTree(root);
            ConfigureLayout(root);

            VisualElement popupRoot = root.Q<VisualElement>("Root") ?? root;
            popupRoot.RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);

            if (ContentHost != null)
            {
                PopulateContent(ContentHost);
            }
        }

        private void OnLostFocus()
        {
            Close();
        }

        protected virtual Vector2 GetPreferredWindowSize()
        {
            return _actualSize;
        }

        protected float ComputeSearchListHeight(int visibleRowCount)
        {
            return ChromePadding + SearchHeight + visibleRowCount * RowHeight;
        }

        internal void Configure(string title, Vector2 pendingMinSize, Rect activatorScreenRect, Rect mainWindowRect)
        {
            _pendingTitle = title ?? string.Empty;
            _pendingMinSize = pendingMinSize;
            _activatorScreenRect = activatorScreenRect;
            _mainWindowRect = mainWindowRect;
            titleContent = new GUIContent(_pendingTitle);
            minSize = pendingMinSize;
        }

        internal void ApplyInitialSize(Vector2 size)
        {
            _actualSize = GetClampedWindowSize(size);
            _pendingWindowRect = GetBestPopupPosition(_activatorScreenRect, _actualSize, _mainWindowRect);
            position = _pendingWindowRect;
        }

        private void OnCustomStyleResolved(CustomStyleResolvedEvent evt)
        {
            if (evt.customStyle.TryGetValue(RowHeightProp, out float row))
            {
                RowHeight = row;
            }

            if (evt.customStyle.TryGetValue(SearchHeightProp, out float search))
            {
                SearchHeight = search;
            }

            if (evt.customStyle.TryGetValue(ChromePaddingProp, out float chrome))
            {
                ChromePadding = chrome;
            }

            RefreshWindowSize();
        }

        protected abstract void PopulateContent(VisualElement host);

        protected void SetTitle(string title)
        {
            _pendingTitle = title ?? string.Empty;
            if (_titleLabel != null)
            {
                _titleLabel.text = _pendingTitle;
            }
        }

        protected float ClampWindowHeight(float desiredHeight)
        {
            float maxHeight = Mathf.Min(MaxWindowHeight,
                _mainWindowRect.height > 0f ? _mainWindowRect.height : MaxWindowHeight);
            return Mathf.Clamp(desiredHeight, _pendingMinSize.y, maxHeight);
        }

        protected float ClampWindowWidth(float desiredWidth)
        {
            float activatorWidth = _activatorScreenRect.width > 0f ? _activatorScreenRect.width : 0f;
            float maxWidth = Mathf.Min(MaxWindowWidth,
                _mainWindowRect.width > 0f ? _mainWindowRect.width : MaxWindowWidth);
            return Mathf.Clamp(Mathf.Max(desiredWidth, activatorWidth), _pendingMinSize.x, maxWidth);
        }

        protected void SetWindowSize(Vector2 desiredSize)
        {
            _actualSize = GetClampedWindowSize(desiredSize);
            Rect currentRect = _pendingWindowRect;
            if (currentRect.width <= 0f || currentRect.height <= 0f)
            {
                currentRect = position;
            }

            _pendingWindowRect = new Rect(currentRect.x, currentRect.y, _actualSize.x, _actualSize.y);
            rootVisualElement.schedule.Execute(() => position = _pendingWindowRect);
        }

        protected void RefreshWindowSize()
        {
            SetWindowSize(GetPreferredWindowSize());
        }

        private Vector2 GetClampedWindowSize(Vector2 desiredSize)
        {
            if (_mainWindowRect.width <= 0f || _mainWindowRect.height <= 0f)
            {
                _mainWindowRect = EditorGUIUtility.GetMainWindowPosition();
            }

            return new Vector2(ClampWindowWidth(desiredSize.x), ClampWindowHeight(desiredSize.y));
        }

        public static Rect GetBestPopupPosition(Rect activatorScreenRect, Vector2 popupSize, Rect containerRect)
        {
            float x = Mathf.Clamp(
                activatorScreenRect.x,
                containerRect.x,
                Mathf.Max(containerRect.x, containerRect.xMax - popupSize.x));

            float belowY = activatorScreenRect.yMax;
            float aboveY = activatorScreenRect.y - popupSize.y;
            float spaceBelow = containerRect.yMax - belowY;
            float spaceAbove = activatorScreenRect.y - containerRect.y;

            bool shouldOpenAbove = spaceBelow < popupSize.y && spaceAbove > spaceBelow;
            float y = shouldOpenAbove
                ? Mathf.Max(containerRect.y, aboveY)
                : Mathf.Min(belowY, containerRect.yMax - popupSize.y);

            return new Rect(x, y, popupSize.x, popupSize.y);
        }

        private void ConfigureLayout(VisualElement root)
        {
            root.style.flexGrow = 1;

            VisualElement popupRoot = root.Q<VisualElement>("Root") ?? root;
            popupRoot.style.flexGrow = 1;

            _titleBar = root.Q<VisualElement>("TitleBar");
            _titleLabel = root.Q<Label>("TitleLabel");
            ContentHost = root.Q<VisualElement>("ContentHost");
            _resizeHandle = root.Q<VisualElement>("ResizeHandle");

            if (_titleBar != null)
            {
                _titleBar.style.display = ShowHeader ? DisplayStyle.Flex : DisplayStyle.None;
                if (ShowHeader && CanDragWindow)
                {
                    RegisterDragHandlers(_titleBar);
                }
            }

            if (_titleLabel != null)
            {
                _titleLabel.text = _pendingTitle;
            }

            if (ContentHost != null)
            {
                ContentHost.style.flexGrow = 1;
            }

            if (_resizeHandle != null)
            {
                if (CanResizeWindow)
                {
                    _resizeHandle.style.width = ResizeHandleSize;
                    _resizeHandle.style.height = ResizeHandleSize;
                    _resizeHandle.style.position = Position.Absolute;
                    _resizeHandle.style.right = 0;
                    _resizeHandle.style.bottom = 0;
                    RegisterResizeHandlers(_resizeHandle);
                }
                else
                {
                    _resizeHandle.style.display = DisplayStyle.None;
                }
            }
        }

        private void RegisterDragHandlers(VisualElement titleBar)
        {
            titleBar.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button != 0)
                {
                    return;
                }

                _dragging = true;
                _dragStartMouse = GUIUtility.GUIToScreenPoint(evt.mousePosition);
                _dragStartPosition = new Vector2(position.x, position.y);
                titleBar.CaptureMouse();
                evt.StopPropagation();
            });
            titleBar.RegisterCallback<MouseMoveEvent>(evt =>
            {
                if (!_dragging)
                {
                    return;
                }

                Vector2 current = GUIUtility.GUIToScreenPoint(evt.mousePosition);
                Vector2 delta = current - _dragStartMouse;
                Rect nextRect = new(_dragStartPosition.x + delta.x, _dragStartPosition.y + delta.y,
                    position.width, position.height);
                _pendingWindowRect = nextRect;
                position = nextRect;
            });
            titleBar.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (!_dragging)
                {
                    return;
                }

                _dragging = false;
                titleBar.ReleaseMouse();
            });
        }

        private void RegisterResizeHandlers(VisualElement handle)
        {
            handle.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button != 0)
                {
                    return;
                }

                _resizing = true;
                _resizeStartMouse = GUIUtility.GUIToScreenPoint(evt.mousePosition);
                _resizeStartSize = new Vector2(position.width, position.height);
                handle.CaptureMouse();
                evt.StopPropagation();
            });
            handle.RegisterCallback<MouseMoveEvent>(evt =>
            {
                if (!_resizing)
                {
                    return;
                }

                Vector2 current = GUIUtility.GUIToScreenPoint(evt.mousePosition);
                Vector2 delta = current - _resizeStartMouse;
                float w = Mathf.Max(minSize.x, _resizeStartSize.x + delta.x);
                float h = Mathf.Max(minSize.y, _resizeStartSize.y + delta.y);
                Rect nextRect = new(position.x, position.y, w, h);
                _pendingWindowRect = nextRect;
                position = nextRect;
            });
            handle.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (!_resizing)
                {
                    return;
                }

                _resizing = false;
                handle.ReleaseMouse();
            });
        }
    }
}
#endif
