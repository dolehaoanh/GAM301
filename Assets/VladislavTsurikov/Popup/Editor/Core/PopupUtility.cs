#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.Popup.Editor
{
    public static class PopupUtility
    {
        public static TPopup OpenPopup<TPopup>(
            Rect activatorRect,
            string title,
            Vector2? size,
            Action<TPopup> initialize,
            Vector2? minSize = null)
            where TPopup : Popup
        {
            Rect screenRect = GUIUtility.GUIToScreenRect(activatorRect);
            Rect mainWindowRect = EditorGUIUtility.GetMainWindowPosition();

            TPopup popup = ScriptableObject.CreateInstance<TPopup>();
            popup.Configure(
                title,
                minSize ?? popup.DefaultMinSize,
                screenRect,
                mainWindowRect);
            initialize?.Invoke(popup);

            popup.ApplyInitialSize(size ?? popup.PreferredSize);
            popup.ShowPopup();
            popup.Focus();
            return popup;
        }
    }
}
#endif
