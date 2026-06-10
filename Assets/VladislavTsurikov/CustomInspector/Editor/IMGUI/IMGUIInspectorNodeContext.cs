#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class IMGUIInspectorNodeContext : InspectorNodeContext
    {
        public Rect Rect { get; set; }
        public float TotalHeight { get; set; }
        public IMGUIRecursiveFieldsDrawer RecursiveFieldsDrawer { get; }
        public Action<object, Rect> DrawNestedFields { get; }

        public IMGUIInspectorNodeContext(
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
            Rect rect,
            float totalHeight,
            IMGUIRecursiveFieldsDrawer recursiveFieldsDrawer,
            Action<object, Rect> drawNestedFields)
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
            Rect = rect;
            TotalHeight = totalHeight;
            RecursiveFieldsDrawer = recursiveFieldsDrawer;
            DrawNestedFields = drawNestedFields;
        }
    }
}
#endif
