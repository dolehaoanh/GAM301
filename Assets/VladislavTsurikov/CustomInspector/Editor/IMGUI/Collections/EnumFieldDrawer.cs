#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class EnumFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override Type DrawerType => typeof(EnumFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType.IsEnum;
        }
    }

    public class EnumFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            return CustomEditorGUI.EnumPopup(rect, label, (Enum)value);
        }
    }
}
#endif
