using System;
using System.Reflection;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public sealed class MaxValueProcessorMatcher : FieldValueProcessorMatcher
    {
        public override Type ProcessorType => typeof(MaxValueProcessor);

        public override bool CanProcess(Attribute attribute)
        {
            return attribute is MaxAttribute;
        }
    }

    public sealed class MaxValueProcessor : FieldValueProcessor
    {
        private float _max;

        public override void Initialize(Attribute attribute)
        {
            base.Initialize(attribute);
            _max = ((MaxAttribute)attribute).Value;
        }

        public override object Process(FieldInfo field, object target, object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is int intValue)
            {
                return Math.Min((int)_max, intValue);
            }

            if (value is float floatValue)
            {
                return Math.Min(_max, floatValue);
            }

            if (value is double doubleValue)
            {
                return Math.Min(_max, doubleValue);
            }

            if (value is long longValue)
            {
                return Math.Min((long)_max, longValue);
            }

            if (value is short shortValue)
            {
                return Math.Min((short)_max, shortValue);
            }

            if (value is byte byteValue)
            {
                byte maxValue = _max <= 0f ? (byte)0 : (byte)_max;
                return Math.Min(maxValue, byteValue);
            }

            if (value is sbyte sbyteValue)
            {
                return Math.Min((sbyte)_max, sbyteValue);
            }

            if (value is uint uintValue)
            {
                uint maxValue = _max <= 0f ? 0u : (uint)_max;
                return uintValue > maxValue ? maxValue : uintValue;
            }

            if (value is ulong ulongValue)
            {
                ulong maxValue = _max <= 0f ? 0ul : (ulong)_max;
                return ulongValue > maxValue ? maxValue : ulongValue;
            }

            if (value is ushort ushortValue)
            {
                ushort maxValue = _max <= 0f ? (ushort)0 : (ushort)_max;
                return Math.Min(maxValue, ushortValue);
            }

            if (value is decimal decimalValue)
            {
                return Math.Min((decimal)_max, decimalValue);
            }

            return value;
        }
    }
}
