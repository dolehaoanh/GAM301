#if UNITY_EDITOR
using System;
using UnityEditor;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
    public class WindowToolStack : NodeStackOnlyDifferentTypes<ToolWindow>
    {
        protected sealed override void OnSetup()
        {
            EditorApplication.update -= DisableToolsIfNecessary;
            EditorApplication.update += DisableToolsIfNecessary;
        }

        protected sealed override void OnCreateElements()
        {
            if (_elementList.Count == 0)
            {
                CreateAllElementTypes();
            }
        }

        protected sealed override void OnDisableStack()
        {
            EditorApplication.update -= DisableToolsIfNecessary;

            Selection.objects = Array.Empty<Object>();
            Tools.current = Tool.None;
        }

        private void DisableToolsIfNecessary()
        {
            if (Tools.current != Tool.None)
            {
                ToolWindow toolWindow = SelectedElement;

                if (toolWindow != null)
                {
                    if (toolWindow.DisableToolIfUnityToolActive())
                    {
                        toolWindow.Selected = false;
                    }
                }
            }
        }

        public void DoSelectedTool()
        {
            if (WindowData.Instance.SelectedTool == null)
            {
                return;
            }

            foreach (ToolWindow item in _elementList)
            {
                if (item.Selected)
                {
                    if (ToolUtility.IsToolSupportSelectedData(item.GetType(), WindowData.Instance.SelectionData))
                    {
                        item.HandleKeyboardEventsInternal();
                        item.DoToolInternal();
                    }

                    return;
                }
            }
        }
    }
}
#endif
