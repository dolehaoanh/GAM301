#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class Vector3FieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override Type DrawerType => typeof(Vector3FieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return field.FieldType == typeof(Vector3);
        }
    }

    public class Vector3FieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            Vector3 vectorValue = value != null ? (Vector3)value : Vector3.zero;
            return CustomEditorGUI.Vector3Field(rect, label, vectorValue);
        }
    }
}
#endif
