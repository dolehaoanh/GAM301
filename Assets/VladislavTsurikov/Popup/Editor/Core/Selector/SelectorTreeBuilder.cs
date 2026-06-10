#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace VladislavTsurikov.Popup.Editor.Selector
{
    public static class SelectorTreeBuilder
    {
        public static List<SelectorItemNode> CreateItemNodes<T>(
            IEnumerable<T> items,
            Func<T, string> getPath,
            Func<T, bool> isEnabled = null)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (getPath == null)
            {
                throw new ArgumentNullException(nameof(getPath));
            }

            List<SelectorItemNode> result = new();
            foreach (T item in items)
            {
                string path = getPath(item) ?? string.Empty;
                result.Add(new SelectorItemNode {
                    Name = SelectorUtility.GetName(path), Path = path, ItemIsEnabled = isEnabled?.Invoke(item) ?? true,
                    SourceObject = item
                });
            }

            return result;
        }

        public static SelectorCategoryNode Build(IEnumerable<SelectorItemNode> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            SelectorCategoryNode root = new() { Depth = -1 };

            foreach (SelectorItemNode item in items)
            {
                SelectorCategoryNode parent = root;
                string[] parts = SelectorUtility.GetCategoryParts(item.Path);

                for (int i = 0; i < parts.Length; i++)
                {
                    parent = FindOrAddCategory(parent, parts[i], i);
                }

                SelectorItemNode itemNode = new() {
                    Name = item.Name,
                    Path = item.Path,
                    Depth = parent.Depth + 1,
                    ItemIsEnabled = item.ItemIsEnabled,
                    SourceObject = item.SourceObject
                };
                itemNode.SetParent(parent);
            }

            SortRecursively(root);
            return root;
        }

        public static List<SelectorNode> FlattenHierarchy(SelectorCategoryNode root)
        {
            List<SelectorNode> output = new();
            foreach (SelectorNode child in root.Children)
            {
                AppendRecursive(child, output);
            }

            return output;
        }

        public static List<SelectorItemNode> FlattenItems(SelectorCategoryNode root)
        {
            List<SelectorItemNode> output = new();
            AppendItemsOnly(root, output);
            return output;
        }

        private static void AppendRecursive(SelectorNode node, List<SelectorNode> output)
        {
            output.Add(node);
            if (node is SelectorCategoryNode categoryNode && !categoryNode.IsCollapsed)
            {
                foreach (SelectorNode child in categoryNode.Children)
                {
                    AppendRecursive(child, output);
                }
            }
        }

        private static void AppendItemsOnly(SelectorCategoryNode node, List<SelectorItemNode> output)
        {
            foreach (SelectorNode child in node.Children)
            {
                if (child is SelectorItemNode itemNode)
                {
                    output.Add(itemNode);
                }
                else if (child is SelectorCategoryNode categoryNode)
                {
                    AppendItemsOnly(categoryNode, output);
                }
            }
        }

        private static SelectorCategoryNode FindOrAddCategory(SelectorCategoryNode parent, string name, int depth)
        {
            foreach (SelectorNode existing in parent.Children)
            {
                if (existing is SelectorCategoryNode categoryNode &&
                    string.Equals(categoryNode.Name, name, StringComparison.Ordinal))
                {
                    return categoryNode;
                }
            }

            SelectorCategoryNode category = new() { Name = name, Depth = depth };
            category.SetParent(parent);
            return category;
        }

        private static void SortRecursively(SelectorCategoryNode node)
        {
            node.Children.Sort((a, b) =>
            {
                bool aIsCategory = a is SelectorCategoryNode;
                bool bIsCategory = b is SelectorCategoryNode;
                if (aIsCategory != bIsCategory)
                {
                    return aIsCategory ? -1 : 1;
                }

                return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
            });

            foreach (SelectorNode child in node.Children)
            {
                if (child is SelectorCategoryNode categoryNode)
                {
                    SortRecursively(categoryNode);
                }
            }
        }
    }
}
#endif
