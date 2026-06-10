using UnityEngine.UIElements;

namespace VladislavTsurikov.UIToolkitUtility.Editor
{
    public abstract class CustomListView<T> : ListView
    {
        protected CustomListView()
        {
            selectionType = SelectionType.Multiple;
            makeItem = CreateItemInternal;
            bindItem = BindItemInternal;
            style.flexGrow = 1;
        }

        protected abstract ListViewItem CreateItem();

        protected abstract void BindItem(ListViewItem item, T data, int index);

        private VisualElement CreateItemInternal() => CreateItem();

        private void BindItemInternal(VisualElement element, int index)
        {
            if (itemsSource == null || index < 0 || index >= itemsSource.Count)
            {
                return;
            }

            if (itemsSource[index] is T data && element is ListViewItem item)
            {
                item.UpdateIndex(index);
                BindItem(item, data, index);
            }
        }
    }
}
