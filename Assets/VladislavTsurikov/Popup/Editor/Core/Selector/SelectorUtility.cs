#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace VladislavTsurikov.Popup.Editor.Selector
{
    public static class SelectorUtility
    {
        public static string GetName(string path)
        {
            string[] parts = GetPathParts(path);
            return parts.Length > 0 ? parts[parts.Length - 1] : string.Empty;
        }

        public static string[] GetCategoryParts(string path)
        {
            string[] parts = GetPathParts(path);
            if (parts.Length <= 1)
            {
                return Array.Empty<string>();
            }

            string[] categories = new string[parts.Length - 1];
            Array.Copy(parts, 0, categories, 0, categories.Length);
            return categories;
        }

        public static string[] GetPathParts(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return Array.Empty<string>();
            }

            string[] raw = path.Split('/');
            List<string> cleaned = new(raw.Length);
            foreach (string part in raw)
            {
                string trimmed = part.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    cleaned.Add(trimmed);
                }
            }

            return cleaned.ToArray();
        }
    }
}
#endif
