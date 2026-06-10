#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class TextureFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override Type DrawerType => typeof(TextureFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(Sprite) || typeof(Texture).IsAssignableFrom(field.FieldType);
        }
    }

    public class TextureFieldDrawer : UIToolkitFieldDrawer
    {
        private const float ObjectFieldSize = 64f;

        public override VisualElement CreateField(string label, Type fieldType, object value,
            Action<object> onValueChanged)
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;

            Label labelElement = new(label);
            labelElement.style.minWidth = 120;

            Type objectType = fieldType;
            if (objectType != typeof(Sprite) && !typeof(Texture).IsAssignableFrom(objectType))
            {
                objectType = typeof(Texture);
            }

            ObjectField field = new() { objectType = objectType, value = value as Object };

            field.style.width = ObjectFieldSize;
            field.style.height = ObjectFieldSize;

            field.RegisterValueChangedCallback(evt => { onValueChanged?.Invoke(evt.newValue); });

            container.Add(labelElement);
            container.Add(field);

            return container;
        }

        public override bool ShouldCreateInstanceIfNull()
        {
            return false;
        }
    }
}
#endif
