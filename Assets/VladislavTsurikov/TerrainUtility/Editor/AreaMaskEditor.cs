#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.TerrainUtility.Runtime;

namespace VladislavTsurikov.TerrainUtility.Editor
{
    [CustomEditor(typeof(AreaMask))]
    public sealed class AreaMaskEditor : UnityEditor.Editor
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new();

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            float fieldsHeight = _fieldsDrawer.GetFieldsHeight(target);
            Rect fieldsRect = EditorGUILayout.GetControlRect(false, fieldsHeight);
            _fieldsDrawer.DrawFields(target, fieldsRect);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
#endif
