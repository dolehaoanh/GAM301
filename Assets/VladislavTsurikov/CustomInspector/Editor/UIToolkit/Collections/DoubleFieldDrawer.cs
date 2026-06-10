#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class DoubleFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override Type DrawerType => typeof(DoubleFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(double);
        }
    }

    public class DoubleFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value,
            Action<object> onValueChanged)
        {
            DoubleField field = new(label) { value = value != null ? (double)value : 0.0 };

            field.RegisterValueChangedCallback(evt => { onValueChanged?.Invoke(evt.newValue); });

            return field;
        }
    }
}
#endif
