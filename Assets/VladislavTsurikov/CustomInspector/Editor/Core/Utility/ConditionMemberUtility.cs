using System.Reflection;
using UnityEngine;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public static class ConditionMemberUtility
    {
        public static bool TryGetConditionValue(object target, string memberName, out object value)
        {
            value = null;
            if (target == null || string.IsNullOrWhiteSpace(memberName))
            {
                return false;
            }

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo field = target.GetType().GetField(memberName, bindingFlags);

            if (field == null)
            {
                PropertyInfo property = target.GetType().GetProperty(memberName, bindingFlags);
                if (property == null)
                {
                    return false;
                }

                value = property.GetValue(target);
                return true;
            }

            value = field.GetValue(target);
            return true;
        }

        public static bool IsTruthy(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is bool boolValue)
            {
                return boolValue;
            }

            if (value is Object unityObject)
            {
                return unityObject != null;
            }

            return true;
        }
    }
}
