#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class GradientFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override Type DrawerType => typeof(GradientFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(Gradient);
        }
    }

    public class GradientFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value,
            Action<object> onValueChanged)
        {
            GradientField field = new(label) { value = value as Gradient ?? new Gradient() };

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
