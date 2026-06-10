#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class Vector2IntFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override Type DrawerType => typeof(Vector2IntFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(Vector2Int);
        }
    }

    public class Vector2IntFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value,
            Action<object> onValueChanged)
        {
            Vector2IntField field = new(label) { value = value != null ? (Vector2Int)value : Vector2Int.zero };

            field.RegisterValueChangedCallback(evt => { onValueChanged?.Invoke(evt.newValue); });

            return field;
        }
    }
}
#endif
