#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.Popup.Editor.Selector;

namespace VladislavTsurikov.Popup.Editor
{
    public static class SelectorPopup
    {
        public static void Open<T>(
            Rect rectActivator,
            string title,
            IEnumerable<T> items,
            Func<T, string> displayNameFunc,
            Action<T> onSelect,
            bool showSearch = true)
        {
            AddPopup.Open(
                rectActivator,
                title,
                items,
                displayNameFunc,
                onSelect);
        }
    }
}
#endif
