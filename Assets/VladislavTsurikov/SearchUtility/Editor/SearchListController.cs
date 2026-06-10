using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.SearchUtility.Editor
{
    public class SearchListController<T>
    {
        private readonly List<T> _filteredItems = new();
        private readonly ListView _listView;
        private readonly SearchView _searchView;
        private bool _ignoreQueryChanged;
        private IList<T> _itemsSource;

        private Action<VisualElement, T, int> _bindItem;
        private Func<VisualElement> _makeItem;

        public Func<T, string, bool> MatchesSearch;
        public Action<string> OnQueryChanged;
        public Action OnRefreshed;
        public Action<T> OnItemActivated;
        public Action<IReadOnlyList<T>> OnSelectionChanged;

        public IReadOnlyList<T> FilteredItems => _filteredItems;
        public string Query => _searchView.Query;

        public IList<T> ItemsSource
        {
            get => _itemsSource;
            set
            {
                _itemsSource = value;
                Refresh();
            }
        }

        public Func<VisualElement> MakeItem
        {
            get => _makeItem;
            set
            {
                _makeItem = value;
                if (_makeItem != null)
                {
                    _listView.makeItem = MakeItemInternal;
                }
            }
        }

        public Action<VisualElement, T, int> BindItem
        {
            get => _bindItem;
            set
            {
                _bindItem = value;
                if (_bindItem != null)
                {
                    _listView.bindItem = BindItemInternal;
                }
            }
        }

        public SearchListController(SearchView searchView, ListView listView)
        {
            _searchView = searchView ?? throw new ArgumentNullException(nameof(searchView));
            _listView = listView ?? throw new ArgumentNullException(nameof(listView));

            _searchView.OnQueryChanged = OnQueryChangedInternal;
            _searchView.SearchField.RegisterCallback<KeyDownEvent>(OnSearchKeyDown);
            _listView.selectionChanged += OnSelectionChangedInternal;
#if UNITY_2022_2_OR_NEWER
            _listView.itemsChosen += OnItemsChosenInternal;
#else
            _listView.onItemsChosen += OnItemsChosenInternal;
#endif
        }

        public void Refresh()
        {
            RefreshFilteredItems();
            _listView.itemsSource = _filteredItems;
            _listView.Rebuild();
            RestoreSelection();
            OnRefreshed?.Invoke();
        }

        public void SetQueryWithoutNotify(string value)
        {
            _ignoreQueryChanged = true;
            _searchView.SetQueryWithoutNotify(value);
            _ignoreQueryChanged = false;
            Refresh();
        }

        public void FocusSearch()
        {
            _searchView.FocusSearch();
        }

        public void SubmitSelection()
        {
            int index = _listView.selectedIndex;
            if (index < 0 && _filteredItems.Count > 0)
            {
                index = 0;
            }

            if (index < 0 || index >= _filteredItems.Count)
            {
                return;
            }

            OnItemActivated?.Invoke(_filteredItems[index]);
        }

        public void MoveSelection(int delta)
        {
            if (_filteredItems.Count == 0)
            {
                return;
            }

            int current = _listView.selectedIndex;
            int next = Mathf.Clamp(current + delta, 0, _filteredItems.Count - 1);
            if (current < 0)
            {
                next = 0;
            }

            _listView.selectedIndex = next;
            _listView.ScrollToItem(next);
        }

        public void SetSelectedIndex(int index, bool scrollTo = true)
        {
            if (index < 0 || index >= _filteredItems.Count)
            {
                _listView.ClearSelection();
                return;
            }

            _listView.selectedIndex = index;
            if (scrollTo)
            {
                _listView.ScrollToItem(index);
            }
        }

        private void OnSearchKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.DownArrow:
                    MoveSelection(1);
                    evt.StopPropagation();
                    break;
                case KeyCode.UpArrow:
                    MoveSelection(-1);
                    evt.StopPropagation();
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    SubmitSelection();
                    evt.StopPropagation();
                    break;
                case KeyCode.Home:
                    SetSelectedIndex(0);
                    evt.StopPropagation();
                    break;
                case KeyCode.End:
                    SetSelectedIndex(_filteredItems.Count - 1);
                    evt.StopPropagation();
                    break;
                case KeyCode.PageDown:
                    MoveSelection(10);
                    evt.StopPropagation();
                    break;
                case KeyCode.PageUp:
                    MoveSelection(-10);
                    evt.StopPropagation();
                    break;
            }
        }

        private VisualElement MakeItemInternal()
        {
            return _makeItem != null ? _makeItem.Invoke() : new Label();
        }

        private void BindItemInternal(VisualElement element, int index)
        {
            if (_bindItem == null || index < 0 || index >= _filteredItems.Count)
            {
                return;
            }

            _bindItem.Invoke(element, _filteredItems[index], index);
        }

        private void OnQueryChangedInternal(string query)
        {
            Refresh();

            if (_ignoreQueryChanged)
            {
                return;
            }

            OnQueryChanged?.Invoke(query);
        }

        private void OnSelectionChangedInternal(IEnumerable<object> selectedItems)
        {
            OnSelectionChanged?.Invoke(selectedItems.OfType<T>().ToList());
        }

        private void OnItemsChosenInternal(IEnumerable<object> chosen)
        {
            foreach (object item in chosen)
            {
                if (item is T typedItem)
                {
                    OnItemActivated?.Invoke(typedItem);
                }

                break;
            }
        }

        private void RefreshFilteredItems()
        {
            _filteredItems.Clear();
            if (_itemsSource == null)
            {
                return;
            }

            string query = Query;
            foreach (T item in _itemsSource)
            {
                if (string.IsNullOrEmpty(query) || IsSearchMatch(item, query))
                {
                    _filteredItems.Add(item);
                }
            }
        }

        private bool IsSearchMatch(T item, string query)
        {
            if (MatchesSearch != null)
            {
                return MatchesSearch.Invoke(item, query);
            }

            string source = item?.ToString() ?? string.Empty;
            return source.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RestoreSelection()
        {
            HashSet<T> selectedSet = new(_filteredItems.Where(item => item is ISelected { IsSelected: true }));
            int[] indices = _filteredItems
                .Select((item, index) => new { item, index })
                .Where(entry => selectedSet.Contains(entry.item))
                .Select(entry => entry.index)
                .ToArray();

            _listView.SetSelectionWithoutNotify(indices);
        }
    }
}
