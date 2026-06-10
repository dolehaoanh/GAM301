using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class InfoBoxAttribute : Attribute
    {
        public string Message { get; }
        public InfoBoxMessageType MessageType { get; }
        public string MessageMemberName { get; }

        public string VisibleIfMemberName { get; set; }

        public InfoBoxAttribute(string message, InfoBoxMessageType messageType = InfoBoxMessageType.Info)
        {
            Message = message;
            MessageType = messageType;
            MessageMemberName = null;
            VisibleIfMemberName = null;
        }

        public InfoBoxAttribute(string messageMemberName, InfoBoxMessageType messageType, bool isDynamic)
        {
            Message = null;
            MessageType = messageType;
            MessageMemberName = isDynamic ? messageMemberName : null;
            Message = isDynamic ? null : messageMemberName;
            VisibleIfMemberName = null;
        }

        public string GetMessage(object target)
        {
            if (!string.IsNullOrWhiteSpace(Message))
            {
                return Message;
            }

            if (string.IsNullOrWhiteSpace(MessageMemberName))
            {
                return string.Empty;
            }

            Type type = target.GetType();

            FieldInfo field = type.GetField(MessageMemberName,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            if (field != null)
            {
                return field.GetValue(target)?.ToString() ?? string.Empty;
            }

            PropertyInfo property = type.GetProperty(MessageMemberName,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            if (property != null)
            {
                return property.GetValue(target)?.ToString() ?? string.Empty;
            }

            MethodInfo method = type.GetMethod(MessageMemberName,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null);

            if (method != null)
            {
                return method.Invoke(target, null)?.ToString() ?? string.Empty;
            }

            return string.Empty;
        }

        public bool IsVisible(object target)
        {
            if (string.IsNullOrWhiteSpace(VisibleIfMemberName))
            {
                return true;
            }

            Type type = target.GetType();

            FieldInfo field = type.GetField(VisibleIfMemberName,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            if (field != null)
            {
                return IsTruthy(field.GetValue(target));
            }

            PropertyInfo property = type.GetProperty(VisibleIfMemberName,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            if (property != null)
            {
                return IsTruthy(property.GetValue(target));
            }

            MethodInfo method = type.GetMethod(VisibleIfMemberName,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null);

            if (method != null)
            {
                return IsTruthy(method.Invoke(target, null));
            }

            return true;
        }

        private bool IsTruthy(object value)
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

    public enum InfoBoxMessageType
    {
        None,
        Info,
        Warning,
        Error
    }
}
