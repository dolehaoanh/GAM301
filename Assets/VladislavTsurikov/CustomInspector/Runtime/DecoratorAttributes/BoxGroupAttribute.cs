using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class BoxGroupAttribute : GroupAttribute
    {
        public bool ShowLabel { get; }
        public BoxGroupAttribute(string groupPath, bool showLabel = true) : base(groupPath) => ShowLabel = showLabel;
    }
}
