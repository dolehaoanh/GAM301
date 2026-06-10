#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;

namespace VladislavTsurikov.CustomInspector.Editor.Collections
{
    public sealed class ListFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override Type DrawerType => typeof(ListFieldDrawer);

        public override bool CanDraw(FieldInfo field)
        {
            if (field.FieldType.IsArray)
            {
                return true;
            }

            if (!field.FieldType.IsGenericType)
            {
                return false;
            }

            if (typeof(IList).IsAssignableFrom(field.FieldType))
            {
                return true;
            }

            return field.FieldType.GetInterface("IList`1") != null;
        }
    }

    public sealed class ListFieldDrawer : IMGUIFieldDrawer
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new(
            new List<Type>()
        );

        private IMGUIFieldDrawer _elementDrawer;
        private Type _elementType;
        private FieldInfo _field;
        private GUIContent _label;
        private IList _list;

        private ReorderableList _reorderableList;
        private object _target;

        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            IList list = value as IList;
            if (list == null && field.FieldType.IsArray)
            {
                list = Array.CreateInstance(field.FieldType.GetElementType(), 0);
                field.SetValue(target, list);
            }

            _field = field;
            _target = target;
            Setup(list, field.FieldType, label);
            _reorderableList.DoList(rect);

            return _list;
        }

        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            if (value is not IList list)
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if (_reorderableList == null || !ReferenceEquals(_list, list))
            {
                Setup(list, field.FieldType, _label ?? GUIContent.none);
            }

            return _reorderableList != null
                ? _reorderableList.GetHeight()
                : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private void Setup(IList list, Type fieldType, GUIContent label)
        {
            _label = label;

            if (_reorderableList == null)
            {
                _list = list;
                _elementType = GetElementType(fieldType);
                _elementDrawer = FieldDrawerResolver<IMGUIFieldDrawer>.CreateDrawer(_elementType);

                _reorderableList = new ReorderableList(_list, _elementType, true, true, true, true);
                _reorderableList.drawHeaderCallback = DrawHeader;
                _reorderableList.elementHeightCallback = GetElementHeight;
                _reorderableList.drawElementCallback = DrawElement;
                _reorderableList.onAddCallback = OnAdd;
                _reorderableList.onRemoveCallback = OnRemove;

                return;
            }

            if (!ReferenceEquals(_list, list))
            {
                _list = list;
                _reorderableList.list = _list;
            }

            _elementType = GetElementType(fieldType);
            _elementDrawer = FieldDrawerResolver<IMGUIFieldDrawer>.CreateDrawer(_elementType);
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, _label);
        }

        private float GetElementHeight(int index)
        {
            if (_elementType == null)
            {
                return EditorGUIUtility.singleLineHeight + 4;
            }

            object element = index >= 0 && index < _list.Count ? _list[index] : null;
            if (_elementDrawer != null)
            {
                return _elementDrawer.GetFieldsHeight(_target, _field, element) + 4;
            }

            if (element == null)
            {
                return EditorGUIUtility.singleLineHeight + 4;
            }

            return _fieldsDrawer.GetFieldsHeight(element) + 4;
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index < 0 || index >= _list.Count)
            {
                return;
            }

            Rect contentRect = new(rect.x, rect.y + 1, rect.width, rect.height - 2);

            object element = _list[index];

            if (_elementDrawer != null)
            {
                object newElement = _elementDrawer.Draw(
                    new Rect(contentRect.x, contentRect.y, contentRect.width, contentRect.height),
                    GUIContent.none,
                    _field,
                    _target,
                    element);
                _list[index] = newElement;
                return;
            }

            if (element == null)
            {
                EditorGUI.LabelField(
                    new Rect(contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight),
                    "Element is null");
                return;
            }

            float height = _fieldsDrawer.GetFieldsHeight(element);
            Rect fieldsRect = new(contentRect.x, contentRect.y, contentRect.width, height);
            _fieldsDrawer.DrawFields(element, fieldsRect);

            _list[index] = element;
        }

        private void OnAdd(ReorderableList list)
        {
            object newElement = FieldUtility.GetOrCreateTypeInstance(_elementType, _elementDrawer);
            if (_field.FieldType.IsArray)
            {
                ResizeArray(_list.Count + 1);
                _list[_list.Count - 1] = newElement;
                GUI.changed = true;
                return;
            }

            _list.Add(newElement);
        }

        private void OnRemove(ReorderableList list)
        {
            if (list.index < 0 || list.index >= _list.Count)
            {
                return;
            }

            if (_field.FieldType.IsArray)
            {
                Array resizedArray = Array.CreateInstance(_elementType, _list.Count - 1);
                int targetIndex = 0;
                for (int sourceIndex = 0; sourceIndex < _list.Count; sourceIndex++)
                {
                    if (sourceIndex == list.index)
                    {
                        continue;
                    }

                    resizedArray.SetValue(_list[sourceIndex], targetIndex++);
                }

                SetList(resizedArray);
                list.index = Mathf.Clamp(list.index, 0, _list.Count - 1);
                GUI.changed = true;
                return;
            }

            _list.RemoveAt(list.index);
        }

        private void ResizeArray(int size)
        {
            Array resizedArray = Array.CreateInstance(_elementType, size);
            int copyCount = Mathf.Min(_list.Count, size);
            for (int i = 0; i < copyCount; i++)
            {
                resizedArray.SetValue(_list[i], i);
            }

            SetList(resizedArray);
        }

        private void SetList(IList list)
        {
            _list = list;
            _field.SetValue(_target, _list);
            if (_reorderableList != null)
            {
                _reorderableList.list = _list;
            }
        }

        private static Type GetElementType(Type fieldType)
        {
            if (fieldType.IsArray)
            {
                return fieldType.GetElementType();
            }

            return fieldType.GetGenericArguments()[0];
        }
    }
}
#endif
