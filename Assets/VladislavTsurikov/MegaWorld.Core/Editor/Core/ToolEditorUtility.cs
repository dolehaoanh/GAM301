#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.Core
{
    public static class ToolEditorUtility
    {
        public static bool DrawWarningAboutUnsupportedResourceType(SelectionData selectionData, Type toolType)
        {
            if (selectionData.SelectedData.HasOneSelectedGroup())
            {
                if (!ToolUtility.IsToolSupportSelectedResourcesType(toolType, selectionData))
                {
                    Group group = selectionData.SelectedData.SelectedGroup;
                    List<Type> supportedResourceTypes = ToolUtility.GetSupportedPrototypeTypes(toolType);
                    string message = "Unsupported Resource Type\n" +
                                     $"This group uses {group.GetPrototypeTypeName()}.\n" +
                                     $"{GetToolName(toolType)} supports {FormatSupportedTypes(supportedResourceTypes)}.";

                    CustomEditorGUILayout.HelpBox(message, MessageType.Error);

                    return false;
                }

                return true;
            }

            if (!ToolUtility.IsToolSupportSelectedMultipleTypes(toolType, selectionData))
            {
                CustomEditorGUILayout.HelpBox(
                    "Unsupported Selection\nThis tool does not support multiple selected groups.", MessageType.Error);
                return false;
            }

            return true;
        }

        private static string GetToolName(Type toolType)
        {
            NameAttribute nameAttribute = toolType.GetAttribute<NameAttribute>();
            return nameAttribute == null ? toolType.Name : nameAttribute.Name.Split('/').Last();
        }

        private static string GetPrototypeTypeName(Type prototypeType)
        {
            NameAttribute nameAttribute = prototypeType.GetAttribute<NameAttribute>();
            return nameAttribute == null ? prototypeType.Name : nameAttribute.Name.Split('/').Last();
        }

        private static string FormatSupportedTypes(IReadOnlyList<Type> supportedResourceTypes)
        {
            List<string> names = supportedResourceTypes
                .Select(GetPrototypeTypeName)
                .ToList();

            return names.Count switch {
                0 => "no resource types",
                1 => names[0],
                2 => $"{names[0]} and {names[1]}",
                _ => string.Join(", ", names.Take(names.Count - 1)) + ", and " + names.Last()
            };
        }
    }
}
#endif
