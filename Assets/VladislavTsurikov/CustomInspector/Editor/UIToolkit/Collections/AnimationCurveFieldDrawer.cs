#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class AnimationCurveFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override Type DrawerType => typeof(AnimationCurveFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(AnimationCurve);
        }
    }

    public class AnimationCurveFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value,
            Action<object> onValueChanged)
        {
            CurveField field = new(label) { value = value as AnimationCurve ?? new AnimationCurve() };

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
