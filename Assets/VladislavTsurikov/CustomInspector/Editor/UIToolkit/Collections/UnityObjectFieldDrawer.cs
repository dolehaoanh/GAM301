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
    public sealed class UnityObjectFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override Type DrawerType => typeof(UnityObjectFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return typeof(Object).IsAssignableFrom(field.FieldType)
                   && field.FieldType != typeof(Sprite)
                   && !typeof(Texture).IsAssignableFrom(field.FieldType);
        }
    }

    public class UnityObjectFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value,
            Action<object> onValueChanged)
        {
            ObjectField field = new(label) { objectType = fieldType, value = value as Object };

            field.RegisterValueChangedCallback(evt => { onValueChanged?.Invoke(evt.newValue); });

            return field;
        }

        public override bool ShouldCreateInstanceIfNull()
        {
            return false;
        }
    }
}
#endif
