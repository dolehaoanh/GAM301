#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class RangeFloatFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override IReadOnlyList<Type> AttributeTypes => new[] { typeof(RangeAttribute) };
        public override Type DrawerType => typeof(RangeFloatFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(float) && field.GetCustomAttribute<RangeAttribute>() != null;
        }
    }

    public sealed class RangeFloatFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            float floatValue = value != null ? (float)value : 0f;
            RangeAttribute rangeAttribute = field.GetCustomAttribute<RangeAttribute>();
            if (rangeAttribute == null)
            {
                return CustomEditorGUI.FloatField(rect, label, floatValue);
            }

            return CustomEditorGUI.Slider(rect, label, floatValue, rangeAttribute.min, rangeAttribute.max);
        }
    }
}
#endif
