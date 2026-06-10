using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class GroupAttribute : Attribute
    {
        public string GroupPath { get; }

        public int Order { get; set; } = 0;
        protected GroupAttribute(string groupPath) => GroupPath = groupPath ?? string.Empty;
    }
}
