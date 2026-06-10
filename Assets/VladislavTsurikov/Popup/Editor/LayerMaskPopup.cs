#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.SearchUtility.Editor;

namespace VladislavTsurikov.Popup.Editor
{
    public sealed class LayerMaskPopup : Popup
    {
        private const float CharWidth = 8f;
        private const float CheckmarkWidth = 18f;
        private const float WidthPadding = 36f;

        private readonly List<LayerItem> _items = new();
        private int _mask;
        private Action _onClosed;
        private Action<int> _onMaskChanged;
        private SearchListController<LayerItem> _searchController;
        private SearchView _searchView;

        protected override bool ShowHeader => false;
        protected override bool CanDragWindow => false;
        protected override bool CanResizeWindow => false;

        private void OnDisable()
        {
            _onClosed?.Invoke();
        }

        protected override Vector2 GetPreferredWindowSize()
        {
            int visibleRowCount = Mathf.Max(1, _searchController?.FilteredItems.Count ?? _items.Count);
            return new Vector2(GetWindowWidth(), ComputeSearchListHeight(visibleRowCount));
        }

        public static void Open(
            Rect rectActivator,
            int currentMask,
            Action<int> onMaskChanged,
            Action onClosed = null)
        {
            List<LayerItem> items = BuildLayerItems();
            PopupUtility.OpenPopup<LayerMaskPopup>(
                rectActivator,
                "Layers",
                null,
                popup =>
                {
                    popup._mask = currentMask;
                    popup._onMaskChanged = onMaskChanged;
                    popup._onClosed = onClosed;
                    popup._items.AddRange(items);
                });
        }

        protected override void PopulateContent(VisualElement host)
        {
            _searchView = new SearchView { PlaceholderText = "Search layers..." };
            ListView listView = new() {
                selectionType = SelectionType.None,
                fixedItemHeight = RowHeight,
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly
            };
            listView.style.flexGrow = 1;

            _searchController = new SearchListController<LayerItem>(_searchView, listView) {
                MakeItem = CreateLayerRow,
                BindItem = BindLayerRow,
                MatchesSearch = (item, query) =>
                    item != null && item.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0,
                OnQueryChanged = _ => RefreshWindowSize(),
                ItemsSource = _items
            };

            host.Add(_searchView);
            host.Add(listView);
            RefreshWindowSize();
            _searchController.FocusSearch();
        }

        private static List<LayerItem> BuildLayerItems()
        {
            List<LayerItem> items = new();
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (string.IsNullOrEmpty(layerName))
                {
                    continue;
                }

                items.Add(new LayerItem { Index = i, Name = layerName });
            }

            return items;
        }

        private VisualElement CreateLayerRow()
        {
            VisualElement row = new() {
                style = {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    minHeight = RowHeight,
                    paddingLeft = 6,
                    paddingRight = 6
                }
            };

            Label label = new()
                { name = "LayerName", style = { fontSize = 11, flexGrow = 1, unityTextAlign = TextAnchor.MiddleLeft } };

            Label checkmark = new() {
                name = "Checkmark",
                style = {
                    flexShrink = 0,
                    width = CheckmarkWidth,
                    minWidth = CheckmarkWidth,
                    unityTextAlign = TextAnchor.MiddleRight,
                    fontSize = 11
                }
            };

            row.RegisterCallback<ClickEvent>(_ =>
            {
                if (row.userData is LayerItem layerItem)
                {
                    SetLayerSelected(layerItem.Index, !IsLayerSelected(layerItem.Index));
                    _searchController?.Refresh();
                }
            });

            row.Add(label);
            row.Add(checkmark);
            return row;
        }

        private void BindLayerRow(VisualElement element, LayerItem layerItem, int index)
        {
            element.userData = layerItem;

            Label nameLabel = element.Q<Label>("LayerName");
            if (nameLabel != null)
            {
                nameLabel.text = layerItem.Name;
            }

            Label checkmarkLabel = element.Q<Label>("Checkmark");
            if (checkmarkLabel != null)
            {
                checkmarkLabel.text = IsLayerSelected(layerItem.Index) ? "\u2713" : string.Empty;
            }
        }

        private bool IsLayerSelected(int index)
        {
            return (_mask & (1 << index)) != 0;
        }

        private void SetLayerSelected(int index, bool isSelected)
        {
            if (isSelected)
            {
                _mask |= 1 << index;
            }
            else
            {
                _mask &= ~(1 << index);
            }

            _onMaskChanged?.Invoke(_mask);
        }

        private float GetWindowWidth()
        {
            int maxNameLength = "Search layers...".Length;
            foreach (LayerItem item in _items)
            {
                if (item.Name != null && item.Name.Length > maxNameLength)
                {
                    maxNameLength = item.Name.Length;
                }
            }

            return maxNameLength * CharWidth + CheckmarkWidth + WidthPadding;
        }

        private sealed class LayerItem
        {
            public int Index;
            public string Name;
        }
    }
}
#endif
