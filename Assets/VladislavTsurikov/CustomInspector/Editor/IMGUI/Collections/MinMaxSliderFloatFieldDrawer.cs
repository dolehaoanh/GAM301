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
    public sealed class MinMaxSliderFloatFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override IReadOnlyList<Type> AttributeTypes => new[] { typeof(MinMaxSliderAttribute) };
        public override Type DrawerType => typeof(MinMaxSliderFloatFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            MinMaxSliderAttribute attribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            return field.FieldType == typeof(float) &&
                   attribute != null &&
                   !string.IsNullOrWhiteSpace(attribute.MaxFieldName);
        }
    }

    public sealed class MinMaxSliderFloatFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            float floatValue = value != null ? (float)value : 0f;
            MinMaxSliderAttribute minMaxAttribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            FieldInfo maxField = target.GetType().GetField(minMaxAttribute.MaxFieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            object maxValue = maxField.GetValue(target);
            float maxFloat = maxValue is float floatMax ? floatMax : floatValue;

            CustomEditorGUI.MinMaxSlider(rect, label, ref floatValue, ref maxFloat, minMaxAttribute.Min,
                minMaxAttribute.Max);

            maxField.SetValue(target, maxFloat);
            return floatValue;
        }

        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
