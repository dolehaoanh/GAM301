#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class InspectorNodeContext
    {
        public object Target { get; }
        public FieldInfo Field { get; }
        public string FieldName { get; }
        public string Tooltip { get; }
        public object Value { get; set; }
        public IList<DecoratorDrawer> Decorators { get; }
        public List<FieldVisibilityProcessor> VisibilityProcessors { get; }
        public List<FieldStateProcessor> StateProcessors { get; }
        public List<FieldStyleProcessor> StyleProcessors { get; }
        public List<FieldValueProcessor> ValueProcessors { get; }
        public bool FlowStopped { get; private set; }

        protected InspectorNodeContext(
            object target,
            FieldInfo field,
            string fieldName,
            string tooltip,
            object value,
            IList<DecoratorDrawer> decorators,
            List<FieldVisibilityProcessor> visibilityProcessors,
            List<FieldStateProcessor> stateProcessors,
            List<FieldStyleProcessor> styleProcessors,
            List<FieldValueProcessor> valueProcessors)
        {
            Target = target;
            Field = field;
            FieldName = fieldName;
            Tooltip = tooltip;
            Value = value;
            Decorators = decorators;
            VisibilityProcessors = visibilityProcessors;
            StateProcessors = stateProcessors;
            StyleProcessors = styleProcessors;
            ValueProcessors = valueProcessors;
            FlowStopped = false;
        }

        public void StopFlow()
        {
            FlowStopped = true;
        }

        public void ResetFlow()
        {
            FlowStopped = false;
        }
    }
}
#endif
