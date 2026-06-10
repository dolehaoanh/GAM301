using System;
using System.Reflection;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public sealed class MinValueProcessorMatcher : FieldValueProcessorMatcher
    {
        public override Type ProcessorType => typeof(MinValueProcessor);

        public override bool CanProcess(Attribute attribute)
        {
            return attribute is MinAttribute;
        }
    }

    public sealed class MinValueProcessor : FieldValueProcessor
    {
        private float _min;

        public override void Initialize(Attribute attribute)
        {
            base.Initialize(attribute);
            _min = ((MinAttribute)attribute).Value;
        }

        public override object Process(FieldInfo field, object target, object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is int intValue)
            {
                return Math.Max((int)_min, intValue);
            }

            if (value is float floatValue)
            {
                return Math.Max(_min, floatValue);
            }

            if (value is double doubleValue)
            {
                return Math.Max(_min, doubleValue);
            }

            if (value is long longValue)
            {
                return Math.Max((long)_min, longValue);
            }

            if (value is short shortValue)
            {
                return Math.Max((short)_min, shortValue);
            }

            if (value is byte byteValue)
            {
                byte minValue = _min <= 0f ? (byte)0 : (byte)_min;
                return Math.Max(minValue, byteValue);
            }

            if (value is sbyte sbyteValue)
            {
                return Math.Max((sbyte)_min, sbyteValue);
            }

            if (value is uint uintValue)
            {
                uint minValue = _min <= 0f ? 0u : (uint)_min;
                return uintValue < minValue ? minValue : uintValue;
            }

            if (value is ulong ulongValue)
            {
                ulong minValue = _min <= 0f ? 0ul : (ulong)_min;
                return ulongValue < minValue ? minValue : ulongValue;
            }

            if (value is ushort ushortValue)
            {
                ushort minValue = _min <= 0f ? (ushort)0 : (ushort)_min;
                return Math.Max(minValue, ushortValue);
            }

            if (value is decimal decimalValue)
            {
                return Math.Max((decimal)_min, decimalValue);
            }

            return value;
        }
    }
}
