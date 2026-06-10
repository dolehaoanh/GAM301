using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UIToolkitUtility.Editor
{
    public abstract class ListViewItem : VisualElement
    {
        public Label ItemIndexLabel { get; }
        public VisualElement ItemContentContainer { get; }
        public Button ItemRemoveButton { get; }

        public bool ShowItemIndex
        {
            set => ItemIndexLabel.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public bool ShowRemoveButton
        {
            set => ItemRemoveButton.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public Action OnRemoveClicked;

        protected ListViewItem()
        {
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                UIToolkitUtilityEditorPath.Path + "/ListViewItem.uss");
            if (styleSheet != null)
            {
                styleSheets.Add(styleSheet);
            }

            AddToClassList("list-view-item");

            ItemIndexLabel = new Label();
            ItemIndexLabel.AddToClassList("list-view-item__index");
            ItemIndexLabel.style.display = DisplayStyle.None;
            Add(ItemIndexLabel);

            ItemContentContainer = new VisualElement();
            ItemContentContainer.AddToClassList("list-view-item__content");
            Add(ItemContentContainer);

            ItemRemoveButton = new Button(() => OnRemoveClicked?.Invoke());
            ItemRemoveButton.text = "−";
            ItemRemoveButton.style.display = DisplayStyle.None;
            Add(ItemRemoveButton);
        }

        internal void UpdateIndex(int index)
        {
            ItemIndexLabel.text = index.ToString();
        }
    }
}
