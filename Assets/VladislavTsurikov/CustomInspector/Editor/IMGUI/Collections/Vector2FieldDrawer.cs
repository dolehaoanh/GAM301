#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class Vector2FieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override Type DrawerType => typeof(Vector2FieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(Vector2);
        }
    }

    public class Vector2FieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            Vector2 vectorValue = value != null ? (Vector2)value : Vector2.zero;
            return CustomEditorGUI.Vector2Field(rect, label, vectorValue);
        }
    }
}
#endif
