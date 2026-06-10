#if UNITY_EDITOR
using System;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList
{
    /// <summary>
    ///     Describes a single entry for the "add type" selector popup.
    ///     Returned by ReorderableListStackEditor.BuildAddItem and equivalents.
    /// </summary>
    public sealed class AddPopupItem
    {
        /// <summary>Row is active and clickable when true, greyed out when false.</summary>
        public bool IsEnabled = true;

        /// <summary>Invoked when the user selects this item.</summary>
        public Action OnPick;

        /// <summary>Slash-separated display path (e.g. "Stats/Modify/Dice").</summary>
        public string Path;

        /// <summary>Type being offered.</summary>
        public Type Type;
    }
}
#endif
