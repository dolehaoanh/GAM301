#if UNITY_EDITOR
using System;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.TemplatesSystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TemplateAttribute : Attribute
    {
        public readonly string Name;
        public readonly Type[] SupportedResourceTypes;
        public readonly string[] ToolNames;

        public TemplateAttribute(string name, string[] toolNames, Type[] supportedResourceTypes)
        {
            Name = name;
            ToolNames = toolNames ?? Array.Empty<string>();
            SupportedResourceTypes = supportedResourceTypes;
        }

        public bool SupportsTool(Type toolType)
        {
            if (toolType == null || ToolNames.Length == 0)
            {
                return false;
            }

            NameAttribute nameAttribute = toolType.GetAttribute<NameAttribute>();
            if (nameAttribute == null)
            {
                return false;
            }

            string toolName = nameAttribute.Name.Split('/').Last();
            return ToolNames.Contains(toolName);
        }
    }
}
#endif
