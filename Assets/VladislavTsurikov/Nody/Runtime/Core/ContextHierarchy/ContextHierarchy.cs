using System;
using System.Collections.Generic;
using System.Reflection;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.Nody.Runtime.Core
{
    public sealed class ContextHierarchy
    {
        private readonly MethodInfo _buildMethod = typeof(ContextHierarchy).GetMethod(nameof(Build),
            BindingFlags.Instance | BindingFlags.NonPublic);

        public List<object> ContextHierarchyData { get; private set; }

        internal void Build<T>(NodeStack<T> stack, List<object> contextHierarchy = null) where T : Node
        {
            if (stack == null)
            {
                return;
            }

            ContextHierarchyData = contextHierarchy ?? new List<object>();

            foreach (T element in stack.ElementList)
            {
                if (element == null)
                {
                    continue;
                }

                element.SetContextHierarchy(ContextHierarchyData);
                BuildForChildStacks(element, ContextHierarchyData);
            }
        }

        private void BuildForChildStacks(Node element, List<object> parentContext)
        {
            FieldInfo[] fields = element.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fields == null || fields.Length == 0)
            {
                return;
            }

            List<object> childContext = parentContext == null ? new List<object>() : new List<object>(parentContext);
            childContext.Add(element);

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                Type fieldType = field.FieldType;
                if (!IsNodeStackType(fieldType))
                {
                    continue;
                }

                object stackValue = field.GetValue(element);
                if (stackValue == null)
                {
                    continue;
                }

                Type elementType = fieldType.TryGetGenericArgument(typeof(NodeStack<>));
                if (elementType == null)
                {
                    continue;
                }

                MethodInfo genericBuild = _buildMethod.MakeGenericMethod(elementType);
                PropertyInfo contextHierarchyProperty = fieldType.GetProperty("ContextHierarchy",
                    BindingFlags.Instance | BindingFlags.Public);
                if (contextHierarchyProperty == null)
                {
                    continue;
                }

                ContextHierarchy childHierarchy = contextHierarchyProperty.GetValue(stackValue) as ContextHierarchy;
                if (childHierarchy == null)
                {
                    continue;
                }

                genericBuild.Invoke(childHierarchy, new[] { stackValue, childContext });
            }
        }

        private static bool IsNodeStackType(Type type)
        {
            return type.TryGetGenericArgument(typeof(NodeStack<>)) != null;
        }

        public bool MatchesParentHierarchy(Type settingsType)
        {
            if (settingsType == null)
            {
                return false;
            }

            ParentRequiredAttribute parentRequired = settingsType.GetAttribute<ParentRequiredAttribute>();
            if (parentRequired == null || parentRequired.ParentTypes == null || parentRequired.ParentTypes.Length == 0)
            {
                return true;
            }

            if (ContextHierarchyData == null || ContextHierarchyData.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < parentRequired.ParentTypes.Length; i++)
            {
                Type requiredType = parentRequired.ParentTypes[i];
                if (requiredType == null)
                {
                    continue;
                }

                bool found = false;
                for (int j = 0; j < ContextHierarchyData.Count; j++)
                {
                    object entry = ContextHierarchyData[j];
                    if (entry != null && requiredType.IsInstanceOfType(entry))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
