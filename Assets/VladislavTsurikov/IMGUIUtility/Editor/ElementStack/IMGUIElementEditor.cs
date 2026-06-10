#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack
{
    public class IMGUIElementEditor : ElementEditor
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new(
            new List<Type> { typeof(Node), typeof(Element) }
        );

        public virtual void OnGUI()
        {
            if (Target == null)
            {
                EditorGUILayout.LabelField("No target assigned.");
                return;
            }

            float totalHeight = _fieldsDrawer.GetFieldsHeight(Target);

            Rect rect = EditorGUILayout.GetControlRect(false, totalHeight);
            _fieldsDrawer.DrawFields(Target, rect);
        }
    }
}
#endif
