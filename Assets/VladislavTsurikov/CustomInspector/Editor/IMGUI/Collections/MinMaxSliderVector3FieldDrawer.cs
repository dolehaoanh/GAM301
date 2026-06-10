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
    public sealed class MinMaxSliderVector3FieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override IReadOnlyList<Type> AttributeTypes => new[] { typeof(MinMaxSliderAttribute) };
        public override Type DrawerType => typeof(MinMaxSliderVector3FieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            MinMaxSliderAttribute attribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            return field.FieldType == typeof(Vector3) &&
                   attribute != null &&
                   !string.IsNullOrWhiteSpace(attribute.MaxFieldName);
        }
    }

    public sealed class MinMaxSliderVector3FieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            Vector3 vectorValue = (Vector3)value;
            MinMaxSliderAttribute minMaxAttribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            FieldInfo maxField = target.GetType().GetField(minMaxAttribute.MaxFieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            object maxValue = maxField.GetValue(target);
            Vector3 maxVector = maxValue is Vector3 vector ? vector : vectorValue;
            bool isUniform = IsUniformMode(target, minMaxAttribute.UniformToggleFieldName);

            if (isUniform)
            {
                float minScale = vectorValue.x;
                float maxScale = maxVector.x;

                CustomEditorGUI.MinMaxSlider(rect, label, ref minScale, ref maxScale, minMaxAttribute.Min,
                    minMaxAttribute.Max);

                vectorValue = new Vector3(minScale, minScale, minScale);
                maxVector = new Vector3(maxScale, maxScale, maxScale);
            }
            else
            {
                GUIContent minLabel = new(ObjectNames.NicifyVariableName(field.Name));
                float lineHeight = EditorGUIUtility.singleLineHeight;
                float lineAdvance = lineHeight + EditorGUIUtility.standardVerticalSpacing;
                Rect minRect = new(rect.x, rect.y, rect.width, lineHeight);
                vectorValue = EditorGUI.Vector3Field(minRect, minLabel, vectorValue);

                GUIContent maxLabel = new(ObjectNames.NicifyVariableName(maxField.Name));
                Rect maxRect = new(rect.x, rect.y + lineAdvance, rect.width, lineHeight);
                maxVector = EditorGUI.Vector3Field(maxRect, maxLabel, maxVector);
            }

            maxField.SetValue(target, maxVector);
            return vectorValue;
        }

        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            MinMaxSliderAttribute attribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            bool isUniform = IsUniformMode(target, attribute.UniformToggleFieldName);
            if (isUniform)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            float lineAdvance = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return lineAdvance * 2f;
        }

        private static bool IsUniformMode(object target, string uniformToggleFieldName)
        {
            if (target == null || string.IsNullOrWhiteSpace(uniformToggleFieldName))
            {
                return true;
            }

            FieldInfo field = target.GetType().GetField(uniformToggleFieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
            {
                return true;
            }

            return field.GetValue(target) is bool boolValue && boolValue;
        }
    }
}
#endif
