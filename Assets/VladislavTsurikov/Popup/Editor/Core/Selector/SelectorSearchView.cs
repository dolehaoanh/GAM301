#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.SearchUtility.Editor;

namespace VladislavTsurikov.Popup.Editor.Selector
{
    public sealed class SelectorSearchView : VisualElement
    {
        private const string StyleResourcePath = "Selector/SelectorPopup";
        private readonly List<SelectorNode> _currentRows = new();

        private readonly SearchListController<SelectorNode> _searchController;
        private readonly SearchView _searchView;
        private SelectorCategoryNode _rootNode;
        private bool _showFlatSearchResults;

        public Action<SelectorItemNode> OnItemSelected { get; set; }
        public Action<int> OnVisibleRowCountChanged { get; set; }

        public string PlaceholderText
        {
            get => _searchView.PlaceholderText;
            set => _searchView.PlaceholderText = value;
        }

        public int VisibleRowCount => Mathf.Max(1, _searchController.FilteredItems.Count);

        public SelectorSearchView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(StyleResourcePath));

            AddToClassList("selector-view");
            style.flexGrow = 1;

            _searchView = new SearchView();
            _searchView.AddToClassList("selector-view__search");

            ListView listView = new() {
                selectionType = SelectionType.Single,
                fixedItemHeight = 22,
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly
            };
            listView.style.flexGrow = 1;

            _searchController = new SearchListController<SelectorNode>(_searchView, listView) {
                MakeItem = MakeRow,
                BindItem = BindRow,
                MatchesSearch = MatchesSearch,
                OnQueryChanged = OnQueryChanged,
                OnItemActivated = OnRowActivated
            };

            Add(_searchView);
            Add(listView);
        }

        public void SetItems(IEnumerable<SelectorItemNode> items)
        {
            _rootNode = SelectorTreeBuilder.Build(items);
            RebuildRows();
        }

        public void FocusSearch()
        {
            _searchController.FocusSearch();
        }

        private void RebuildRows()
        {
            _currentRows.Clear();
            if (_rootNode == null)
            {
                _searchController.ItemsSource = _currentRows;
                return;
            }

            if (_showFlatSearchResults)
            {
                _currentRows.AddRange(SelectorTreeBuilder.FlattenItems(_rootNode));
            }
            else
            {
                _currentRows.AddRange(SelectorTreeBuilder.FlattenHierarchy(_rootNode));
            }

            _searchController.ItemsSource = _currentRows;
            NotifyVisibleRowCountChanged();
        }

        private void OnQueryChanged(string query)
        {
            bool wasShowingFlatSearchResults = _showFlatSearchResults;
            _showFlatSearchResults = !string.IsNullOrEmpty(query);
            if (_showFlatSearchResults != wasShowingFlatSearchResults)
            {
                RebuildRows();
            }

            if (_showFlatSearchResults && _searchController.FilteredItems.Count > 0)
            {
                _searchController.SetSelectedIndex(0, false);
            }

            NotifyVisibleRowCountChanged();
        }

        private bool MatchesSearch(SelectorNode itemNode, string query)
        {
            if (itemNode is not SelectorItemNode selectorItemNode)
            {
                return false;
            }

            string haystack = selectorItemNode.Path ?? selectorItemNode.Name ?? string.Empty;
            return haystack.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private VisualElement MakeRow()
        {
            VisualElement row = new();
            row.AddToClassList("selector-row");
            row.RegisterCallback<ClickEvent>(OnRowClicked);

            VisualElement indent = new() { name = "Indent" };
            indent.AddToClassList("selector-row__indent");
            row.Add(indent);

            VisualElement expander = new() { name = "Expander" };
            expander.AddToClassList("selector-row__expander");
            expander.pickingMode = PickingMode.Position;
            VisualElement expanderIcon = new() { name = "ExpanderIcon" };
            expanderIcon.AddToClassList("selector-row__expander-icon");
            expander.Add(expanderIcon);
            row.Add(expander);

            Label nameLabel = new() { name = "Name" };
            nameLabel.AddToClassList("selector-row__name");
            row.Add(nameLabel);

            Label pathLabel = new() { name = "Path" };
            pathLabel.AddToClassList("selector-row__path");
            row.Add(pathLabel);

            return row;
        }

        private void BindRow(VisualElement element, SelectorNode node, int _)
        {
            element.userData = node;

            VisualElement indent = element.Q<VisualElement>("Indent");
            VisualElement expander = element.Q<VisualElement>("Expander");
            VisualElement expanderIcon = element.Q<VisualElement>("ExpanderIcon");
            Label nameLabel = element.Q<Label>("Name");
            Label pathLabel = element.Q<Label>("Path");

            indent.style.width = Mathf.Max(0, node.Depth) * 12f;

            element.EnableInClassList("selector-row--category", node is SelectorCategoryNode);
            element.EnableInClassList("selector-row--item", node is SelectorItemNode);
            element.EnableInClassList("selector-row--disabled", !node.IsEnabled);

            if (node is SelectorCategoryNode categoryNode)
            {
                expander.style.display = DisplayStyle.Flex;
                expanderIcon?.EnableInClassList("selector-row__expander-icon--collapsed", categoryNode.IsCollapsed);
                nameLabel.text = categoryNode.Name;
                pathLabel.text = string.Empty;
                pathLabel.style.display = DisplayStyle.None;
            }
            else if (node is SelectorItemNode itemNode)
            {
                expander.style.display = DisplayStyle.None;
                if (expanderIcon != null)
                {
                    expanderIcon.EnableInClassList("selector-row__expander-icon--collapsed", false);
                }

                nameLabel.text = itemNode.Name;
                string[] categories = SelectorUtility.GetCategoryParts(itemNode.Path);
                if (_showFlatSearchResults && categories.Length > 0)
                {
                    pathLabel.text = string.Join("/", categories);
                    pathLabel.style.display = DisplayStyle.Flex;
                }
                else
                {
                    pathLabel.text = string.Empty;
                    pathLabel.style.display = DisplayStyle.None;
                }
            }
        }

        private void OnRowActivated(SelectorNode node)
        {
            ActivateNode(node);
        }

        private void OnRowClicked(ClickEvent evt)
        {
            if (evt.currentTarget is not VisualElement row)
            {
                return;
            }

            if (row.userData is not SelectorNode node)
            {
                return;
            }

            ActivateNode(node);
            evt.StopPropagation();
        }

        private void ActivateNode(SelectorNode node)
        {
            if (node is SelectorCategoryNode categoryNode)
            {
                categoryNode.IsCollapsed = !categoryNode.IsCollapsed;
                RebuildRows();
                return;
            }

            if (node is not SelectorItemNode itemNode || !itemNode.IsEnabled)
            {
                return;
            }

            OnItemSelected?.Invoke(itemNode);
        }

        private void NotifyVisibleRowCountChanged()
        {
            OnVisibleRowCountChanged?.Invoke(VisibleRowCount);
        }
    }
}
#endif
