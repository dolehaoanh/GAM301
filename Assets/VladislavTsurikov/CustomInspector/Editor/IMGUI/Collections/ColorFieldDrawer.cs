#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class ColorFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override Type DrawerType => typeof(ColorFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(Color);
        }
    }

    public class ColorFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            Color colorValue = value != null ? (Color)value : default;
            return CustomEditorGUI.ColorField(rect, label, colorValue);
        }
    }
}
#endif
