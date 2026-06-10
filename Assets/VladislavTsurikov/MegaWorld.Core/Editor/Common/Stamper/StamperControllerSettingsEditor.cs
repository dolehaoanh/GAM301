#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Stamper
{
    [ElementEditor(typeof(StamperControllerSettings))]
    public class StamperControllerSettingsEditor : IMGUIElementEditor
    {
        private readonly GUIContent _autoRespawn = new("Auto Respawn",
            "Allows you to do automatic deletion and then spawn when you changed the settings.");

        private readonly GUIContent _delayAutoSpawn = new("Delay Auto Spawn", "Respawn delay in seconds.");

        private readonly GUIContent _visualisation =
            new("Visualisation", "Allows you to see the Mask Filter Settings visualization.");

        private StamperControllerSettings _stamperControllerSettings => (StamperControllerSettings)Target;

        public override void OnGUI()
        {
            _stamperControllerSettings.Visualisation =
                CustomEditorGUILayout.Toggle(_visualisation, _stamperControllerSettings.Visualisation);

            _stamperControllerSettings.AutoRespawn =
                CustomEditorGUILayout.Toggle(_autoRespawn, _stamperControllerSettings.AutoRespawn);

            if (_stamperControllerSettings.AutoRespawn)
            {
                EditorGUI.indentLevel++;
                _stamperControllerSettings.DelayAutoRespawn = CustomEditorGUILayout.Slider(_delayAutoSpawn,
                    _stamperControllerSettings.DelayAutoRespawn, 0, 3);
                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif
