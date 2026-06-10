using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class MaxAttribute : Attribute
    {
        public float Value { get; }
        public MaxAttribute(float value) => Value = value;
    }
}
