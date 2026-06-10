#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class LayerMaskFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override Type DrawerType => typeof(LayerMaskFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(LayerMask);
        }
    }

    public class LayerMaskFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value,
            Action<object> onValueChanged)
        {
            LayerMask layerMask = value != null ? (LayerMask)value : new LayerMask();
            LayerMaskField field = new(label, layerMask.value);

            field.RegisterValueChangedCallback(evt =>
            {
                LayerMask newLayerMask = evt.newValue;
                onValueChanged?.Invoke(newLayerMask);
            });

            return field;
        }
    }
}
#endif
