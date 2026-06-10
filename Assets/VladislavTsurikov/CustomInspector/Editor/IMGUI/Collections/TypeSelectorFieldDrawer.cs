#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.Popup.Editor;

namespace VladislavTsurikov.ReflectionUtility.Runtime.CustomInspectorIntegration
{
    public sealed class TypeSelectorFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override Type DrawerType => typeof(TypeSelectorFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType.IsGenericType &&
                   field.FieldType.GetGenericTypeDefinition() == typeof(TypeSelector<>);
        }
    }

    public class TypeSelectorFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo fieldInfo, object target, object value)
        {
            if (value == null)
            {
                return null;
            }

            Type baseType = fieldInfo.FieldType.GenericTypeArguments[0];


            // COMMENT OUT CODE CU VA THAY NOI DUNG MOI O DUOI DE FIX LOI ERROR SAU KHI IMPORT
            // dynamic typeReference = Convert.ChangeType(value, fieldInfo.FieldType);

            // if (typeReference == null)
            // {
            //     return value;
            // }

            // Rect labelRect = new(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            // Rect buttonRect = new(rect.x + EditorGUIUtility.labelWidth + 5, rect.y,
            //     rect.width - EditorGUIUtility.labelWidth - 5, EditorGUIUtility.singleLineHeight);

            // EditorGUI.LabelField(labelRect, label);

            // string currentTypeName = typeReference.Type != null ? typeReference.Type.Name : "(None)";
            // if (GUI.Button(buttonRect, currentTypeName))
            // {
            //     SelectorPopup.Open(
            //         buttonRect,
            //         $"Select {baseType.Name}",
            //         GetDerivedTypes(baseType),
            //         type => type.Name,
            //         selectedType => { typeReference.Type = selectedType; });
            // }

            object typeReference = value;
            PropertyInfo typeProperty = fieldInfo.FieldType.GetProperty("Type");

            if (typeReference == null || typeProperty == null)
            {
                return value;
            }

            Rect labelRect = new(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            Rect buttonRect = new(rect.x + EditorGUIUtility.labelWidth + 5, rect.y,
                rect.width - EditorGUIUtility.labelWidth - 5, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, label);

            Type currentType = (Type)typeProperty.GetValue(typeReference);
            string currentTypeName = currentType != null ? currentType.Name : "(None)";
            if (GUI.Button(buttonRect, currentTypeName))
            {
                SelectorPopup.Open(
                    buttonRect,
                    $"Select {baseType.Name}",
                    GetDerivedTypes(baseType),
                    type => type.Name,
                    selectedType => { typeProperty.SetValue(typeReference, selectedType); });
            }

            return value;
        }

        private static IEnumerable<Type> GetDerivedTypes(Type baseType)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type type in assembly.GetTypes())
                {
                    if (baseType.IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        yield return type;
                    }
                }
        }
    }
}
#endif
