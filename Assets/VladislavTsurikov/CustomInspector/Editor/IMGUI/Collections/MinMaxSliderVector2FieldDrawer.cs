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
    public sealed class MinMaxSliderVector2FieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override IReadOnlyList<Type> AttributeTypes => new[] { typeof(MinMaxSliderAttribute) };
        public override Type DrawerType => typeof(MinMaxSliderVector2FieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(Vector2) && field.GetCustomAttribute<MinMaxSliderAttribute>() != null;
        }
    }

    public sealed class MinMaxSliderVector2FieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            Vector2 vectorValue = (Vector2)value;
            MinMaxSliderAttribute minMaxAttribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            float minValue = vectorValue.x;
            float maxValue = vectorValue.y;

            CustomEditorGUI.MinMaxSlider(rect, label, ref minValue, ref maxValue, minMaxAttribute.Min,
                minMaxAttribute.Max);

            return new Vector2(minValue, maxValue);
        }

        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
