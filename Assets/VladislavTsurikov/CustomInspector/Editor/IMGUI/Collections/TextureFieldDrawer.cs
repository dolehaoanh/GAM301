#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class TextureFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override Type DrawerType => typeof(TextureFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(Sprite) || typeof(Texture).IsAssignableFrom(field.FieldType);
        }
    }

    public class TextureFieldDrawer : IMGUIFieldDrawer
    {
        private const float ObjectFieldSize = 50f;

        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            float labelHeight = EditorGUIUtility.singleLineHeight;
            Rect labelRect = new(rect.x, rect.y, labelWidth, labelHeight);
            Rect fieldRect = new(rect.x + labelWidth, rect.y, ObjectFieldSize, ObjectFieldSize);

            EditorGUI.LabelField(labelRect, label);

            Type objectType = field.FieldType;
            if (objectType != typeof(Sprite) && !typeof(Texture).IsAssignableFrom(objectType))
            {
                objectType = typeof(Texture);
            }

            return EditorGUI.ObjectField(fieldRect, (Object)value, objectType, true);
        }

        public override bool ShouldCreateInstanceIfNull()
        {
            return false;
        }

        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            return ObjectFieldSize;
        }
    }
}
#endif
