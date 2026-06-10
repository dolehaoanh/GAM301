#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class IMGUIRecursiveDrawerNode : InspectorNode
    {
        private readonly IMGUIRecursiveFieldsDrawer _recursiveFieldsDrawer;

        public IMGUIRecursiveDrawerNode(IMGUIRecursiveFieldsDrawer recursiveFieldsDrawer) =>
            _recursiveFieldsDrawer = recursiveFieldsDrawer;

        public override void Execute(InspectorNodeContext context)
        {
            if (context is not IMGUIInspectorNodeContext imguiContext)
            {
                return;
            }

            IMGUIRecursiveFieldsDrawer.RecursiveDrawResult result = _recursiveFieldsDrawer.DrawRecursiveFields(
                context.Value,
                context.Field,
                imguiContext.Rect,
                (nestedTarget, nestedRect) => imguiContext.DrawNestedFields?.Invoke(nestedTarget, nestedRect));

            if (!result.IsExpanded)
            {
                context.StopFlow();
            }

            Rect rect = imguiContext.Rect;
            rect.y += result.Height;
            imguiContext.Rect = rect;
            imguiContext.TotalHeight += result.Height;
        }
    }
}
#endif
