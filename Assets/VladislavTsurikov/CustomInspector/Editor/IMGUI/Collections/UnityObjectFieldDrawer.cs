#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class UnityObjectFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override Type DrawerType => typeof(UnityObjectFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            return typeof(Object).IsAssignableFrom(field.FieldType)
                   && field.FieldType != typeof(Sprite)
                   && !typeof(Texture).IsAssignableFrom(field.FieldType);
        }
    }

    public class UnityObjectFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            Type objectType = field.FieldType;

            // Handle case when called from ListFieldDrawer with collection FieldInfo
            if (objectType.IsGenericType && typeof(IList).IsAssignableFrom(objectType))
            {
                objectType = objectType.GetGenericArguments()[0];
            }

            return CustomEditorGUI.ObjectField(rect, label, (Object)value, objectType);
        }

        public override bool ShouldCreateInstanceIfNull()
        {
            return false;
        }
    }
}
#endif
