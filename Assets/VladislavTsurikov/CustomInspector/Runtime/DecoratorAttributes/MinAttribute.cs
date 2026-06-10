using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class MinAttribute : Attribute
    {
        public float Value { get; }
        public MinAttribute(float value) => Value = value;
    }
}
