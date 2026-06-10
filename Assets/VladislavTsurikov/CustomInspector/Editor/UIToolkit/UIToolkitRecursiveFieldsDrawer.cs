#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public class UIToolkitRecursiveFieldsDrawer : RecursiveFieldsDrawer
    {
        public readonly struct RecursiveElementResult
        {
            public VisualElement Element { get; }
            public bool IsExpanded { get; }

            public RecursiveElementResult(VisualElement element, bool isExpanded)
            {
                Element = element;
                IsExpanded = isExpanded;
            }
        }

        public RecursiveElementResult DrawRecursiveFields(
            object value,
            FieldInfo fieldInfo,
            Action<object, VisualElement> drawField)
        {
            VisualElement container = new();

            Foldout foldout = new()
                { text = ObjectNames.NicifyVariableName(fieldInfo.Name), value = GetFoldoutState(value) };

            foldout.RegisterValueChangedCallback(evt => { SetFoldoutState(value, evt.newValue); });

            VisualElement contentContainer = new();
            contentContainer.style.paddingLeft = 15;

            if (foldout.value)
            {
                drawField(value, contentContainer);
            }

            foldout.RegisterValueChangedCallback(evt =>
            {
                contentContainer.Clear();
                if (evt.newValue)
                {
                    drawField(value, contentContainer);
                }
            });

            container.Add(foldout);
            container.Add(contentContainer);

            return new RecursiveElementResult(container, foldout.value);
        }
    }
}
#endif
