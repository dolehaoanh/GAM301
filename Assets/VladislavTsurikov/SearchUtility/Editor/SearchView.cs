using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.SearchUtility.Editor
{
    public class SearchView : VisualElement
    {
        private const string SearchViewResourcePath = "SearchUtility/SearchView";
        private const string SearchViewStyleResourcePath = "SearchUtility/SearchViewStyle";

        private const string PlaceholderHiddenClassName = "searchPlaceholderLabel--hidden";

        private readonly Label _autoCompleteLabel;
        private readonly Label _placeholderLabel;
        private readonly VisualElement _searchContainer;
        private readonly VisualElement _searchIcon;
        private readonly VisualElement _searchTextInput;
        private bool _ignoreQueryChanged;
        private string _placeholderText = "Start typing to search...";

        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = string.IsNullOrWhiteSpace(value) ? "Start typing to search..." : value;
                _placeholderLabel.text = _placeholderText;
                UpdatePlaceholderVisibility();
            }
        }

        public string AutoCompleteText
        {
            get => _autoCompleteLabel.text;
            set
            {
                bool hasText = !string.IsNullOrEmpty(value);
                _autoCompleteLabel.text = value ?? string.Empty;
                _autoCompleteLabel.style.display = hasText ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public TextField SearchField { get; }

        public string Query => (SearchField.value ?? string.Empty).Trim();

        public Action<string> OnQueryChanged { get; set; }

        public SearchView()
        {
            VisualTreeAsset layout = Resources.Load<VisualTreeAsset>(SearchViewResourcePath);
            StyleSheet styleSheet = Resources.Load<StyleSheet>(SearchViewStyleResourcePath);

            if (styleSheet != null)
            {
                styleSheets.Add(styleSheet);
            }

            if (layout == null)
            {
                throw new InvalidOperationException(
                    $"SearchView layout was not found at Resources/{SearchViewResourcePath}.");
            }

            Add(layout.CloneTree());

            _searchContainer = this.Q<VisualElement>("windowSearchBoxVisualContainer");
            _searchIcon = this.Q<VisualElement>("searchIcon");
            SearchField = this.Q<TextField>("searchBox");
            _placeholderLabel = this.Q<Label>("searchPlaceholderLabel");
            _autoCompleteLabel = this.Q<Label>("autoCompleteLabel");

            _searchContainer.AddToClassList(SearchFieldBase<TextField, string>.ussClassName);
            SearchField.AddToClassList(TextInputBaseField<string>.ussClassName);
            _searchTextInput = SearchField.Q(TextInputBaseField<string>.textInputUssName);
            if (SearchField.labelElement != null)
            {
                SearchField.labelElement.style.display = DisplayStyle.None;
                SearchField.labelElement.style.width = 0;
                SearchField.labelElement.style.minWidth = 0;
                SearchField.labelElement.style.marginRight = 0;
                SearchField.labelElement.style.paddingRight = 0;
            }

            ApplySearchIconFallback();

            _placeholderLabel.pickingMode = PickingMode.Ignore;
            _autoCompleteLabel.pickingMode = PickingMode.Ignore;

            SearchField.RegisterValueChangedCallback(OnSearchValueChanged);

            PlaceholderText = _placeholderText;
            AutoCompleteText = null;
            UpdatePlaceholderVisibility();
        }

        public void SetQueryWithoutNotify(string value)
        {
            _ignoreQueryChanged = true;
            SearchField.SetValueWithoutNotify(value ?? string.Empty);
            _ignoreQueryChanged = false;
            UpdatePlaceholderVisibility();
        }

        public void FocusSearch()
        {
            schedule.Execute(() => (_searchTextInput ?? SearchField)?.Focus()).ExecuteLater(0);
        }

        private void OnSearchValueChanged(ChangeEvent<string> evt)
        {
            UpdatePlaceholderVisibility();

            if (_ignoreQueryChanged)
            {
                return;
            }

            OnQueryChanged?.Invoke(evt.newValue?.Trim() ?? string.Empty);
        }

        private void UpdatePlaceholderVisibility()
        {
            bool hasValue = !string.IsNullOrEmpty(SearchField.value);
            _placeholderLabel.EnableInClassList(PlaceholderHiddenClassName, hasValue);
        }

        private void ApplySearchIconFallback()
        {
            if (_searchIcon == null)
            {
                return;
            }

            Texture2D icon = EditorGUIUtility.IconContent("Search Icon").image as Texture2D;
            if (icon != null)
            {
                _searchIcon.style.backgroundImage = new StyleBackground(icon);
            }
        }
    }
}
