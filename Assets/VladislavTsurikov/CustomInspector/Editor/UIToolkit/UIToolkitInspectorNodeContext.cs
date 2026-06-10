#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class UIToolkitInspectorNodeContext : InspectorNodeContext
    {
        public VisualElement Container { get; }
        public UIToolkitRecursiveFieldsDrawer RecursiveFieldsDrawer { get; }
        public Action<object, VisualElement> DrawNestedFields { get; }

        public UIToolkitInspectorNodeContext(
            object target,
            FieldInfo field,
            string fieldName,
            string tooltip,
            object value,
            IList<DecoratorDrawer> decorators,
            List<FieldVisibilityProcessor> visibilityProcessors,
            List<FieldStateProcessor> stateProcessors,
            List<FieldStyleProcessor> styleProcessors,
            List<FieldValueProcessor> valueProcessors,
            VisualElement container,
            UIToolkitRecursiveFieldsDrawer recursiveFieldsDrawer,
            Action<object, VisualElement> drawNestedFields)
            : base(
                target,
                field,
                fieldName,
                tooltip,
                value,
                decorators,
                visibilityProcessors,
                stateProcessors,
                styleProcessors,
                valueProcessors)
        {
            Container = container;
            RecursiveFieldsDrawer = recursiveFieldsDrawer;
            DrawNestedFields = drawNestedFields;
        }
    }
}
#endif
