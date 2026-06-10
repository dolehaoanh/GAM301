#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class MinMaxSliderIntFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override IReadOnlyList<Type> AttributeTypes => new[] { typeof(MinMaxSliderAttribute) };
        public override Type DrawerType => typeof(MinMaxSliderIntFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            MinMaxSliderAttribute attribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            return field.FieldType == typeof(int) &&
                   attribute != null &&
                   !string.IsNullOrWhiteSpace(attribute.MaxFieldName);
        }
    }

    public sealed class MinMaxSliderIntFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            int intValue = value != null ? (int)value : 0;
            MinMaxSliderAttribute minMaxAttribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            FieldInfo maxField = target.GetType().GetField(minMaxAttribute.MaxFieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            object maxValue = maxField.GetValue(target);
            int maxInt = maxValue is int intMax ? intMax : intValue;

            CustomEditorGUI.MinMaxIntSlider(rect, label, ref intValue, ref maxInt, (int)minMaxAttribute.Min,
                (int)minMaxAttribute.Max);

            maxField.SetValue(target, maxInt);
            return intValue;
        }

        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
