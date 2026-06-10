#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Attributes;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    public class TransformStackEditor : ReorderableListStackEditor<TransformComponent, ReorderableListComponentEditor>
    {
        private readonly List<Type> _dontShowTransformTypes = new();
        private readonly bool _useSimpleComponent;

        private NodeStackOnlyDifferentTypes<TransformComponent> ComponentStackOnlyDifferentTypes =>
            (NodeStackOnlyDifferentTypes<TransformComponent>)Stack;

        public TransformStackEditor(GUIContent reorderableListName, TransformComponentStack list,
            List<Type> dontShowTransformTypes, bool useSimpleComponent) : base(reorderableListName, list, true)
        {
            DisplayHeaderText = false;
            _dontShowTransformTypes = dontShowTransformTypes;
            _useSimpleComponent = useSimpleComponent;
        }

        public TransformStackEditor(GUIContent reorderableListName, TransformComponentStack list,
            bool useSimpleComponent) : base(reorderableListName, list, true)
        {
            DisplayHeaderText = false;
            _useSimpleComponent = useSimpleComponent;
        }

        public TransformStackEditor(GUIContent reorderableListName, TransformComponentStack list) : base(
            reorderableListName, list, true)
        {
            DisplayHeaderText = false;
            _useSimpleComponent = false;
        }

        protected override AddPopupItem BuildAddItem(Type settingsType)
        {
            if (_dontShowTransformTypes.Contains(settingsType))
            {
                return null;
            }

            string name = settingsType.GetAttribute<NameAttribute>()?.Name;
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            string path;
            if (_useSimpleComponent)
            {
                if (settingsType.GetAttribute<SimpleAttribute>() == null)
                {
                    return null;
                }

                path = name;
            }
            else
            {
                path = settingsType.GetAttribute<SimpleAttribute>() != null
                    ? "Simple/" + name
                    : "Advanced/" + name;
            }

            return new AddPopupItem {
                Type = settingsType, Path = path, IsEnabled = !ComponentStackOnlyDifferentTypes.HasType(settingsType),
                OnPick = () => ComponentStackOnlyDifferentTypes.CreateIfMissingType(settingsType)
            };
        }
    }
}
#endif
