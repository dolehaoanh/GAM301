#if UNITY_EDITOR
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class UIToolkitRecursiveDrawerNode : InspectorNode
    {
        private readonly UIToolkitRecursiveFieldsDrawer _recursiveFieldsDrawer;

        public UIToolkitRecursiveDrawerNode(UIToolkitRecursiveFieldsDrawer recursiveFieldsDrawer) =>
            _recursiveFieldsDrawer = recursiveFieldsDrawer;

        public override void Execute(InspectorNodeContext context)
        {
            if (context is not UIToolkitInspectorNodeContext uiContext)
            {
                return;
            }

            UIToolkitRecursiveFieldsDrawer.RecursiveElementResult result = _recursiveFieldsDrawer.DrawRecursiveFields(
                context.Value,
                context.Field,
                (nestedTarget, nestedContainer) => uiContext.DrawNestedFields?.Invoke(nestedTarget, nestedContainer));

            if (!result.IsExpanded)
            {
                context.StopFlow();
            }

            uiContext.Container.Add(result.Element);
        }
    }
}
#endif
