#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class RangeIntFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override IReadOnlyList<Type> AttributeTypes => new[] { typeof(RangeAttribute) };
        public override Type DrawerType => typeof(RangeIntFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(int) && field.GetCustomAttribute<RangeAttribute>() != null;
        }
    }

    public sealed class RangeIntFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            int intValue = value != null ? (int)value : 0;
            RangeAttribute rangeAttribute = field.GetCustomAttribute<RangeAttribute>();
            if (rangeAttribute == null)
            {
                return CustomEditorGUI.IntField(rect, label, intValue);
            }

            return CustomEditorGUI.IntSlider(rect, label, intValue, (int)rangeAttribute.min, (int)rangeAttribute.max);
        }
    }
}
#endif
