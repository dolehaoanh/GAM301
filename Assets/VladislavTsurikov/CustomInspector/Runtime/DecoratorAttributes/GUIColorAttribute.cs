using System;
using System.Reflection;
using UnityEngine;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class GUIColorAttribute : Attribute
    {
        public Color Color { get; }
        public string ColorMemberName { get; }

        public GUIColorAttribute(float r, float g, float b, float a = 1f)
        {
            Color = new Color(r, g, b, a);
            ColorMemberName = null;
        }

        public GUIColorAttribute(string colorMemberName)
        {
            Color = Color.white;
            ColorMemberName = colorMemberName;
        }

        public Color GetColor(object target)
        {
            if (string.IsNullOrWhiteSpace(ColorMemberName))
            {
                return Color;
            }

            Type type = target.GetType();

            FieldInfo field = type.GetField(ColorMemberName,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            if (field != null && field.FieldType == typeof(Color))
            {
                return (Color)field.GetValue(target);
            }

            PropertyInfo property = type.GetProperty(ColorMemberName,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            if (property != null && property.PropertyType == typeof(Color))
            {
                return (Color)property.GetValue(target);
            }

            MethodInfo method = type.GetMethod(ColorMemberName,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null);

            if (method != null && method.ReturnType == typeof(Color))
            {
                return (Color)method.Invoke(target, null);
            }

            return Color;
        }
    }
}
