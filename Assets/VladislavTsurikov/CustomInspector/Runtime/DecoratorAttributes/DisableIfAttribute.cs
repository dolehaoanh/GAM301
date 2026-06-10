using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DisableIfAttribute : Attribute
    {
        public string ConditionMemberName { get; }
        public DisableIfAttribute(string conditionMemberName) => ConditionMemberName = conditionMemberName;
    }
}
