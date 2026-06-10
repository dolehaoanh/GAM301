using System;
using UnityEngine.UIElements;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class HelpBoxAttribute : Attribute
    {
        public string Message { get; }
        public HelpBoxMessageType MessageType { get; }

        public HelpBoxAttribute(string message, HelpBoxMessageType messageType = HelpBoxMessageType.Info)
        {
            Message = message;
            MessageType = messageType;
        }
    }
}
