#if UNITY_EDITOR
using System;
using System.Reflection;
using Assemblies.VladislavTsurikov.Nody.Runtime.SingleElementStack;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.SingleElementStackEditor.CustomInspectorIntegration
{
    public sealed class SingleElementStackFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override Type DrawerType => typeof(SingleElementStackFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType.IsGenericType &&
                   field.FieldType.GetGenericTypeDefinition() == typeof(SingleElementStack<>);
        }
    }

    public class SingleElementStackFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            if (value == null)
            {
                return null;
            }

            Type elementType = field.FieldType.GenericTypeArguments[0];

            if (value is not ISingleElementStack singleElementStack)
            {
                return value;
            }

            Rect labelRect = new(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            Rect buttonRect = new(rect.x + EditorGUIUtility.labelWidth + 5, rect.y,
                rect.width - EditorGUIUtility.labelWidth - 5, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, label);

            object currentElement = singleElementStack.GetObjectElement();
            string currentElementName = currentElement != null ? currentElement.GetType().Name : "No Element";

            if (GUI.Button(buttonRect, currentElementName))
            {
                ShowAddMenu(singleElementStack, elementType);
            }

            return singleElementStack;
        }

        private void ShowAddMenu(ISingleElementStack stack, Type elementType)
        {
            GenericMenu menu = new();

            foreach (Type type in TypeHierarchyHelper.GetDerivedTypes(elementType))
            {
                string typeName = type.Name;

                menu.AddItem(new GUIContent(typeName), false, () => stack.ReplaceElement(type));
            }

            menu.ShowAsContext();
        }
    }
}
#endif
