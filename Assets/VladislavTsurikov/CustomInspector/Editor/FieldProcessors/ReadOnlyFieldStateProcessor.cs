using System;
using System.Reflection;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.FieldProcessors
{
    public sealed class ReadOnlyFieldStateProcessorMatcher : FieldStateProcessorMatcher
    {
        public override Type ProcessorType => typeof(ReadOnlyFieldStateProcessor);

        public override bool CanProcess(Attribute attribute)
        {
            return attribute is ReadOnlyAttribute;
        }
    }

    public sealed class ReadOnlyFieldStateProcessor : FieldStateProcessor
    {
        public override void Apply(FieldInfo field, object target, FieldState state)
        {
            if (state == null)
            {
                return;
            }

            state.IsReadOnly = true;
        }
    }
}
