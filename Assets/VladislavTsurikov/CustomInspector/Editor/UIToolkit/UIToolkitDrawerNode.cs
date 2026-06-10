#if UNITY_EDITOR
using System.Reflection;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class UIToolkitDrawerNode : InspectorNode
    {
        private readonly UIToolkitFieldDrawer _drawer;
        private readonly UIToolkitInspectorFieldsDrawer _owner;

        public UIToolkitDrawerNode(UIToolkitInspectorFieldsDrawer owner, UIToolkitFieldDrawer drawer)
        {
            _owner = owner;
            _drawer = drawer;
        }

        public override void Execute(InspectorNodeContext context)
        {
            if (context is not UIToolkitInspectorNodeContext uiContext)
            {
                return;
            }

            FieldInfo field = context.Field;
            object target = context.Target;
            object value = context.Value;

            object processedValue = _owner.ApplyProcessorsAndAssignIfNeeded(
                field,
                target,
                value,
                value,
                context.ValueProcessors,
                false);
            context.Value = processedValue;

            VisualElement fieldElement = _drawer.CreateField(
                context.FieldName,
                field.FieldType,
                processedValue,
                newValue =>
                {
                    _owner.ApplyProcessorsAndAssignIfNeeded(
                        field,
                        target,
                        newValue,
                        newValue,
                        context.ValueProcessors,
                        true);
                });

            using (_owner.CreateFieldPresentationScope(
                       field,
                       target,
                       context.StateProcessors,
                       context.StyleProcessors,
                       fieldElement))
            {
                if (!string.IsNullOrEmpty(context.Tooltip))
                {
                    fieldElement.tooltip = context.Tooltip;
                }

                uiContext.Container.Add(fieldElement);
            }
        }
    }
}
#endif
