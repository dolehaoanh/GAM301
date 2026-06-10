#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class InspectorFieldsDrawer<TFieldDrawer, TDecoratorDrawer>
        where TFieldDrawer : FieldDrawer
        where TDecoratorDrawer : DecoratorDrawer
    {
        protected sealed class ProcessedField
        {
            public FieldInfo Field { get; }
            public string FieldName { get; }
            public TFieldDrawer Drawer { get; }
            public List<TDecoratorDrawer> Decorators { get; }
            public List<DecoratorDrawer> DecoratorsBase { get; }
            public List<FieldVisibilityProcessor> VisibilityProcessors { get; }
            public List<FieldStateProcessor> StateProcessors { get; }
            public List<FieldStyleProcessor> StyleProcessors { get; }
            public List<FieldValueProcessor> ValueProcessors { get; }
            public string Tooltip { get; }
            public object Value { get; set; }
            public FieldGraph Graph { get; set; }

            public ProcessedField(
                FieldInfo field,
                string fieldName,
                TFieldDrawer drawer,
                List<TDecoratorDrawer> decorators,
                List<FieldVisibilityProcessor> visibilityProcessors,
                List<FieldStateProcessor> stateProcessors,
                List<FieldStyleProcessor> styleProcessors,
                List<FieldValueProcessor> valueProcessors,
                string tooltip)
            {
                Field = field;
                FieldName = fieldName;
                Drawer = drawer;
                Decorators = decorators;
                DecoratorsBase = ConvertDecorators(decorators);
                VisibilityProcessors = visibilityProcessors;
                StateProcessors = stateProcessors;
                StyleProcessors = styleProcessors;
                ValueProcessors = valueProcessors;
                Tooltip = tooltip;
            }

            private static List<DecoratorDrawer> ConvertDecorators(List<TDecoratorDrawer> decorators)
            {
                if (decorators == null || decorators.Count == 0)
                {
                    return new List<DecoratorDrawer>();
                }

                List<DecoratorDrawer> result = new(decorators.Count);
                foreach (TDecoratorDrawer decorator in decorators)
                {
                    result.Add(decorator);
                }

                return result;
            }
        }

        private readonly BindingFlags _bindingFlags;
        private readonly Dictionary<Type, List<ProcessedField>> _cachedFields = new();
        private readonly List<Type> _excludedDeclaringTypes;
        private readonly bool _excludeInternal;

        protected InspectorFieldsDrawer(
            List<Type> excludedDeclaringTypes = null,
            bool excludeInternal = true,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            _excludedDeclaringTypes = excludedDeclaringTypes ?? new List<Type>();
            _excludeInternal = excludeInternal;
            _bindingFlags = bindingFlags;
        }

        protected IEnumerable<ProcessedField> GetProcessedFields(object target)
        {
            List<ProcessedField> processedFields = GetOrCreateProcessedFields(target.GetType());

            foreach (ProcessedField processedField in processedFields)
            {
                if (!IsFieldVisible(processedField.Field, target, processedField.VisibilityProcessors))
                {
                    continue;
                }

                TFieldDrawer drawer = processedField.Drawer;

                object value = drawer == null || drawer.ShouldCreateInstanceIfNull()
                    ? FieldUtility.GetOrCreateFieldInstance(processedField.Field, target)
                    : processedField.Field.GetValue(target);

                processedField.Value = value;

                yield return processedField;
            }
        }

        private List<ProcessedField> GetOrCreateProcessedFields(Type targetType)
        {
            if (_cachedFields.TryGetValue(targetType, out List<ProcessedField> processedFields))
            {
                return processedFields;
            }

            FieldInfo[] fields = FieldUtility.GetSerializableFields(
                targetType,
                _bindingFlags,
                _excludeInternal,
                _excludedDeclaringTypes.ToArray()
            );

            fields = fields.OrderBy(field =>
            {
                OrderAttribute orderAttribute = field.GetCustomAttribute<OrderAttribute>();
                return orderAttribute?.Order ?? int.MaxValue;
            }).ToArray();

            processedFields = new List<ProcessedField>(fields.Length);

            foreach (FieldInfo field in fields)
            {
                TFieldDrawer drawer = FieldDrawerResolver<TFieldDrawer>.CreateDrawer(field);
                string fieldName = FieldUtility.GetFieldLabel(field);

                List<TDecoratorDrawer> decorators = DecoratorDrawerResolver<TDecoratorDrawer>.CreateDrawers(field);
                List<FieldVisibilityProcessor> visibilityProcessors =
                    FieldVisibilityProcessorResolver.CreateProcessors(field);
                List<FieldStateProcessor> stateProcessors = FieldStateProcessorResolver.CreateProcessors(field);
                List<FieldStyleProcessor> styleProcessors = FieldStyleProcessorResolver.CreateProcessors(field);
                List<FieldValueProcessor> valueProcessors = FieldValueProcessorResolver.CreateProcessors(field);

                TooltipAttribute tooltipAttribute = field.GetCustomAttribute<TooltipAttribute>();
                string tooltip = tooltipAttribute?.tooltip ?? "";

                ProcessedField processedField = new(
                    field,
                    fieldName,
                    drawer,
                    decorators,
                    visibilityProcessors,
                    stateProcessors,
                    styleProcessors,
                    valueProcessors,
                    tooltip);

                processedField.Graph = CreateFieldGraph(processedField);

                processedFields.Add(processedField);
            }

            _cachedFields[targetType] = processedFields;

            return processedFields;
        }

        protected bool IsFieldVisible(
            FieldInfo field,
            object target,
            List<FieldVisibilityProcessor> visibilityProcessors)
        {
            if (visibilityProcessors == null || visibilityProcessors.Count == 0)
            {
                return true;
            }

            foreach (FieldVisibilityProcessor processor in visibilityProcessors)
            {
                if (!processor.IsVisible(field, target))
                {
                    return false;
                }
            }

            return true;
        }

        protected abstract FieldGraph CreateFieldGraph(ProcessedField processedField);

        protected FieldState GetFieldState(
            FieldInfo field,
            object target,
            List<FieldStateProcessor> stateProcessors)
        {
            FieldState state = new();
            if (stateProcessors == null || stateProcessors.Count == 0)
            {
                return state;
            }

            foreach (FieldStateProcessor processor in stateProcessors)
            {
                processor.Apply(field, target, state);
            }

            return state;
        }

        protected FieldStyle GetFieldStyle(
            FieldInfo field,
            object target,
            List<FieldStyleProcessor> styleProcessors)
        {
            FieldStyle style = new();
            if (styleProcessors == null || styleProcessors.Count == 0)
            {
                return style;
            }

            foreach (FieldStyleProcessor processor in styleProcessors)
            {
                processor.Apply(field, target, style);
            }

            return style;
        }

        internal FieldPresentationScope CreateFieldPresentationScope(
            FieldInfo field,
            object target,
            List<FieldStateProcessor> stateProcessors,
            List<FieldStyleProcessor> styleProcessors,
            object fieldElement)
        {
            FieldState state = GetFieldState(field, target, stateProcessors);
            FieldStyle style = GetFieldStyle(field, target, styleProcessors);
            return CreateFieldPresentationScope(state, style, fieldElement);
        }

        protected abstract FieldPresentationScope CreateFieldPresentationScope(
            FieldState state,
            FieldStyle style,
            object fieldElement);

        protected object ApplyValueProcessors(
            FieldInfo field,
            object target,
            object value,
            List<FieldValueProcessor> valueProcessors)
        {
            if (valueProcessors == null || valueProcessors.Count == 0)
            {
                return value;
            }

            object processedValue = value;
            foreach (FieldValueProcessor processor in valueProcessors)
            {
                processedValue = processor.Process(field, target, processedValue);
            }

            return processedValue;
        }

        protected object ApplyValueProcessorsAndCheckChange(
            FieldInfo field,
            object target,
            object value,
            object originalValue,
            List<FieldValueProcessor> valueProcessors,
            out bool hasValueChanged)
        {
            object processedValue = ApplyValueProcessors(field, target, value, valueProcessors);
            hasValueChanged = !Equals(originalValue, processedValue);
            return processedValue;
        }

        internal object ApplyProcessorsAndAssignIfNeeded(
            FieldInfo field,
            object target,
            object value,
            object originalValue,
            List<FieldValueProcessor> valueProcessors,
            bool forceAssign)
        {
            bool hasValueChanged;
            object processedValue = ApplyValueProcessorsAndCheckChange(
                field,
                target,
                value,
                originalValue,
                valueProcessors,
                out hasValueChanged);

            if (forceAssign || hasValueChanged)
            {
                field.SetValue(target, processedValue);
            }

            return processedValue;
        }
    }
}
#endif
