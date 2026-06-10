#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.Popup.Editor.Selector
{
    public sealed class AddPopup : Popup
    {
        private const float WidthPadding = 48f;
        private const float CharWidth = 8f;

        private List<SelectorItemNode> _items;
        private Action<object> _onSelect;
        private string _placeholder;
        private SelectorSearchView _view;

        protected override bool ShowHeader => false;
        protected override bool CanDragWindow => false;
        protected override bool CanResizeWindow => false;

        protected override Vector2 GetPreferredWindowSize()
        {
            int visibleRowCount = _view?.VisibleRowCount ?? GetInitialVisibleRowCount();
            return new Vector2(GetWindowWidth(), ComputeSearchListHeight(visibleRowCount));
        }

        public static AddPopup Open<T>(
            Rect activatorRect,
            string title,
            IEnumerable<T> items,
            Func<T, string> getPath,
            Action<T> onSelect,
            Func<T, bool> isEnabled = null,
            string placeholder = null,
            Vector2? size = null)
        {
            List<SelectorItemNode> data = SelectorTreeBuilder.CreateItemNodes(items, getPath, isEnabled);
            return PopupUtility.OpenPopup<AddPopup>(
                activatorRect,
                title,
                size,
                popup =>
                {
                    popup._items = data;
                    popup._onSelect = raw => onSelect?.Invoke((T)raw);
                    popup._placeholder = placeholder;
                });
        }

        protected override void PopulateContent(VisualElement host)
        {
            _view = new SelectorSearchView();
            if (!string.IsNullOrEmpty(_placeholder))
            {
                _view.PlaceholderText = _placeholder;
            }

            _view.SetItems(_items);
            _view.OnItemSelected = OnItemSelected;
            _view.OnVisibleRowCountChanged = _ => RefreshWindowSize();
            host.Add(_view);
            RefreshWindowSize();
            _view.FocusSearch();
        }

        private void OnItemSelected(SelectorItemNode data)
        {
            _onSelect?.Invoke(data?.SourceObject);
            Close();
        }

        private int GetInitialVisibleRowCount()
        {
            SelectorCategoryNode root = SelectorTreeBuilder.Build(_items);
            return Mathf.Max(1, SelectorTreeBuilder.FlattenHierarchy(root).Count);
        }

        private float GetWindowWidth()
        {
            int maxLength = 0;

            if (!string.IsNullOrEmpty(_placeholder))
            {
                maxLength = _placeholder.Length;
            }

            foreach (SelectorItemNode item in _items)
            {
                string name = SelectorUtility.GetName(item.Path);
                if (!string.IsNullOrEmpty(name) && name.Length > maxLength)
                {
                    maxLength = name.Length;
                }

                foreach (string category in SelectorUtility.GetCategoryParts(item.Path))
                {
                    if (category.Length > maxLength)
                    {
                        maxLength = category.Length;
                    }
                }
            }

            return maxLength * CharWidth + WidthPadding;
        }
    }
}
#endif
