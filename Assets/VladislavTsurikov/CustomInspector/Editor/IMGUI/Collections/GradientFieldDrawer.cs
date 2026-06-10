#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class GradientFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override Type DrawerType => typeof(GradientFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(Gradient);
        }
    }

    public class GradientFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            Gradient gradient = value as Gradient ?? new Gradient();
            return CustomEditorGUI.GradientField(rect, label, gradient);
        }

        public override bool ShouldCreateInstanceIfNull()
        {
            return false;
        }
    }
}
#endif
