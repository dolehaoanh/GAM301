#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.ScatterSystem
{
    [ElementEditor(typeof(RandomGrid))]
    public class RandomGridEditor : ReorderableListComponentEditor
    {
        private RandomGrid _randomGrid;

        public override void OnEnable()
        {
            _randomGrid = (RandomGrid)Target;
        }

        public override void OnGUI(Rect rect, int index)
        {
            _randomGrid.RandomisationType = (RandomisationType)CustomEditorGUI.EnumPopup(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Randomisation Type"), _randomGrid.RandomisationType);
            rect.y += EditorGUIUtility.singleLineHeight;

            if (_randomGrid.RandomisationType == RandomisationType.Sphere)
            {
                _randomGrid.Vastness =
                    CustomEditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Vastness"), _randomGrid.Vastness, 0f, 1f);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            float distance = _randomGrid.GridStep.x;

            _randomGrid.UniformGrid =
                CustomEditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Uniform Grid"), _randomGrid.UniformGrid);
            rect.y += EditorGUIUtility.singleLineHeight;

            if (_randomGrid.UniformGrid)
            {
                distance = CustomEditorGUI.Slider(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Distance"), distance, 0.1f, 50f);
                rect.y += EditorGUIUtility.singleLineHeight;

                _randomGrid.GridAngle =
                    CustomEditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Angle"), _randomGrid.GridAngle, 0, 360);
                rect.y += EditorGUIUtility.singleLineHeight;

                _randomGrid.GridStep = Vector2.Max(new Vector2(0.5f, 0.5f), new Vector2(distance, distance));
                rect.y += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                _randomGrid.GridStep = Vector2.Max(new Vector2(0.1f, 0.1f),
                    CustomEditorGUI.Vector2Field(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Step"), _randomGrid.GridStep));
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;

                _randomGrid.GridAngle =
                    CustomEditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Angle"), _randomGrid.GridAngle, 0, 360);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            _randomGrid.FailureRate =
                CustomEditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Failure Rate Value (%)"), _randomGrid.FailureRate, 0f, 100f);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            if (_randomGrid.RandomisationType == RandomisationType.Sphere)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;

            if (_randomGrid.UniformGrid)
            {
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif
