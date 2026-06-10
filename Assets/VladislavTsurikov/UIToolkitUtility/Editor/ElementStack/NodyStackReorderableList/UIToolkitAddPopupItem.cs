#if UNITY_EDITOR
using System;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.UIToolkitReorderableList
{
    /// <summary>
    ///     Describes a single entry for the UIToolkit "add type" selector popup.
    ///     Returned by ReorderableListStackEditor.BuildAddItem.
    /// </summary>
    public sealed class UIToolkitAddPopupItem
    {
        public bool IsEnabled = true;
        public Action OnPick;
        public string Path;
        public Type Type;
    }
}
#endif
