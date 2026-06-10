#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class ReactivePropertyFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override Type DrawerType => typeof(ReactivePropertyFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return IsReactivePropertyType(field.FieldType);
        }

        private static bool IsReactivePropertyType(Type type)
        {
            return type != null &&
                   type.IsGenericType &&
                   type.GetGenericTypeDefinition().FullName == "UniRx.ReactiveProperty`1";
        }
    }

    public sealed class ReactivePropertyFieldDrawer : IMGUIFieldDrawer
    {
        private static class TypeOnlyField<T>
        {
            public static T Value;
        }

        private static readonly Dictionary<Type, PropertyInfo> ValuePropertyCache = new();
        private static readonly Dictionary<Type, FieldInfo> TypeOnlyFields = new();
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new();
        private IMGUIFieldDrawer _elementDrawer;
        private Type _elementType;

        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            if (field == null)
            {
                EditorGUI.LabelField(rect, label, new GUIContent("Field is null"));
                return value;
            }

            if (value == null)
            {
                EditorGUI.LabelField(rect, label, new GUIContent("ReactiveProperty is null"));
                return value;
            }

            Type fieldType = field.FieldType;
            _elementType = fieldType.GetGenericArguments()[0];
            _elementDrawer = FieldDrawerResolver<IMGUIFieldDrawer>.CreateDrawer(_elementType);
            PropertyInfo valueProperty = GetValueProperty(fieldType);
            if (valueProperty == null)
            {
                EditorGUI.LabelField(rect, label, new GUIContent("ReactiveProperty.Value not found"));
                return value;
            }

            object currentValue = valueProperty.GetValue(value);
            object newValue = DrawValue(rect, label, currentValue, target);

            if (!Equals(currentValue, newValue))
            {
                valueProperty.SetValue(value, newValue);
            }

            return value;
        }

        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            if (field == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            if (value == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            _elementType = field.FieldType.GetGenericArguments()[0];
            _elementDrawer = FieldDrawerResolver<IMGUIFieldDrawer>.CreateDrawer(_elementType);

            object currentValue = GetValueProperty(field.FieldType)?.GetValue(value);

            if (_elementDrawer != null)
            {
                return _elementDrawer.GetFieldsHeight(
                    target,
                    GetTypeOnlyField(_elementType),
                    currentValue);
            }

            if (currentValue == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return _fieldsDrawer.GetFieldsHeight(currentValue);
        }

        private static PropertyInfo GetValueProperty(Type reactiveType)
        {
            if (ValuePropertyCache.TryGetValue(reactiveType, out PropertyInfo property))
            {
                return property;
            }

            property = reactiveType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            ValuePropertyCache[reactiveType] = property;
            return property;
        }

        private object DrawValue(Rect rect, GUIContent label, object value, object target)
        {
            if (value == null)
            {
                value = FieldUtility.GetOrCreateTypeInstance(_elementType, _elementDrawer);
            }

            if (_elementDrawer != null)
            {
                return _elementDrawer.Draw(
                    rect,
                    label,
                    GetTypeOnlyField(_elementType),
                    target,
                    value);
            }

            if (value == null)
            {
                EditorGUI.LabelField(rect, label, new GUIContent("Value is null"));
                return value;
            }

            _fieldsDrawer.DrawFields(value, rect);
            return value;
        }

        private static FieldInfo GetTypeOnlyField(Type fieldType)
        {
            if (TypeOnlyFields.TryGetValue(fieldType, out FieldInfo field))
            {
                return field;
            }

            Type holderType = typeof(TypeOnlyField<>).MakeGenericType(fieldType);
            field = holderType.GetField(nameof(TypeOnlyField<int>.Value), BindingFlags.Public | BindingFlags.Static);
            TypeOnlyFields[fieldType] = field;
            return field;
        }
    }
}
#endif
