#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class IMGUIInspectorFieldsDrawer : InspectorFieldsDrawer<IMGUIFieldDrawer, IMGUIDecoratorDrawer>
    {
        private readonly IMGUIRecursiveFieldsDrawer _imguiRecursiveFieldsDrawer = new();

        private float _totalHeight;

        public IMGUIInspectorFieldsDrawer(
            List<Type> excludedDeclaringTypes = null,
            bool excludeInternal = true,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            : base(excludedDeclaringTypes, excludeInternal, bindingFlags)
        {
        }

        public void DrawFields(object target, Rect rect, int? elementIndex = null)
        {
            if (target == null)
            {
                EditorGUI.LabelField(rect, "Target is null");
                return;
            }

            _totalHeight = 0;

            DrawFieldsRecursive(target, rect, elementIndex);

            ButtonDrawer.DrawButtons(target, ref rect);
        }

        private void DrawFieldsRecursive(object target, Rect rect, int? elementIndex)
        {
            if (target == null)
            {
                return;
            }

            foreach (ProcessedField processedField in GetProcessedFields(target))
            {
                IMGUIInspectorNodeContext context = new(
                    target,
                    processedField.Field,
                    processedField.FieldName,
                    processedField.Tooltip,
                    processedField.Value,
                    processedField.DecoratorsBase,
                    processedField.VisibilityProcessors,
                    processedField.StateProcessors,
                    processedField.StyleProcessors,
                    processedField.ValueProcessors,
                    rect,
                    _totalHeight,
                    _imguiRecursiveFieldsDrawer,
                    (nestedTarget, nestedRect) => DrawFieldsRecursive(nestedTarget, nestedRect, elementIndex));

                processedField.Graph.Execute(context);

                rect = context.Rect;
                _totalHeight = context.TotalHeight;
            }
        }

        public float GetFieldsHeight(object target, int? elementIndex = null)
        {
            if (target == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            _totalHeight = 0;
            CalculateFieldsHeight(target, elementIndex);
            _totalHeight += ButtonDrawer.GetButtonsHeight(target);
            return _totalHeight;
        }

        private void CalculateFieldsHeight(object target, int? elementIndex)
        {
            if (target == null)
            {
                return;
            }

            foreach (ProcessedField processedField in GetProcessedFields(target))
            {
                foreach (IMGUIDecoratorDrawer decorator in processedField.Decorators)
                {
                    _totalHeight += decorator.GetHeight(processedField.Field, target);
                }

                if (processedField.Drawer != null)
                {
                    _totalHeight += processedField.Drawer.GetFieldsHeight(
                        target,
                        processedField.Field,
                        processedField.Value);
                }
                else
                {
                    _totalHeight += EditorGUIUtility.singleLineHeight;
                }
            }
        }

        protected override FieldPresentationScope CreateFieldPresentationScope(
            FieldState state,
            FieldStyle style,
            object fieldElement)
        {
            return new IMGUIFieldPresentationScope(state, style);
        }

        protected override FieldGraph CreateFieldGraph(ProcessedField processedField)
        {
            FieldGraph graph = new();
            InspectorNode previousNode = null;

            foreach (IMGUIDecoratorDrawer decorator in processedField.Decorators)
            {
                IMGUIDecoratorNode node = new(decorator);
                graph.AddNode(node);
                if (previousNode != null)
                {
                    graph.Connect(previousNode, node);
                }

                previousNode = node;
            }

            InspectorNode finalNode = processedField.Drawer != null
                ? new IMGUIDrawerNode(this, processedField.Drawer)
                : new IMGUIRecursiveDrawerNode(_imguiRecursiveFieldsDrawer);

            graph.AddNode(finalNode);
            if (previousNode != null)
            {
                graph.Connect(previousNode, finalNode);
            }

            return graph;
        }
    }
}
#endif
