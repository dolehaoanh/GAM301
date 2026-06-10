using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class HideIfAttribute : Attribute
    {
        public string ConditionMemberName { get; }
        public HideIfAttribute(string conditionMemberName) => ConditionMemberName = conditionMemberName;
    }
}
