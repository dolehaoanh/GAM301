#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.MegaWorld.Editor.AdvancedBrushTool
{
    [DontDrawFoldout]
    [ElementEditor(typeof(AdvancedObjectBrushSettings))]
    public class AdvancedObjectBrushElementEditor : IMGUIElementEditor
    {
        private AdvancedObjectBrushSettings _advancedObjectBrushSettings;

        public override void OnEnable()
        {
            _advancedObjectBrushSettings = (AdvancedObjectBrushSettings)Target;
        }

        public override void OnGUI()
        {
            _advancedObjectBrushSettings.EnableFailureRateOnMouseDrag = CustomEditorGUILayout.Toggle(
                new GUIContent("Enable Failure Rate On Mouse Drag"),
                _advancedObjectBrushSettings.EnableFailureRateOnMouseDrag);

            if (_advancedObjectBrushSettings.EnableFailureRateOnMouseDrag)
            {
                _advancedObjectBrushSettings.FailureRateOnMouseDrag = CustomEditorGUILayout.Slider(
                    new GUIContent("Failure Rate On Mouse Drag"),
                    _advancedObjectBrushSettings.FailureRateOnMouseDrag, 0, 100);
            }
        }
    }
}
#endif
