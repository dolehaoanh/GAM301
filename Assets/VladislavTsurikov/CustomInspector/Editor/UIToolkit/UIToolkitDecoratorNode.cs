#if UNITY_EDITOR
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class UIToolkitDecoratorNode : InspectorNode
    {
        private readonly UIToolkitDecoratorDrawer _decorator;

        public UIToolkitDecoratorNode(UIToolkitDecoratorDrawer decorator) => _decorator = decorator;

        public override void Execute(InspectorNodeContext context)
        {
            if (context is not UIToolkitInspectorNodeContext uiContext)
            {
                return;
            }

            VisualElement decoratorElement = _decorator.CreateElement(context.Field, context.Target);
            if (decoratorElement != null)
            {
                uiContext.Container.Add(decoratorElement);
            }
        }
    }
}
#endif
