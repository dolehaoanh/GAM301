using System;
using System.Reflection;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.FieldProcessors
{
    public sealed class HideIfVisibilityProcessorMatcher : FieldVisibilityProcessorMatcher
    {
        public override Type ProcessorType => typeof(HideIfVisibilityProcessor);

        public override bool CanProcess(Attribute attribute)
        {
            return attribute is HideIfAttribute;
        }
    }

    public sealed class HideIfVisibilityProcessor : FieldVisibilityProcessor
    {
        private HideIfAttribute _attribute;

        public override void Initialize(Attribute attribute)
        {
            base.Initialize(attribute);
            _attribute = attribute as HideIfAttribute;
        }

        public override bool IsVisible(FieldInfo field, object target)
        {
            if (_attribute == null)
            {
                return true;
            }

            if (!ConditionMemberUtility.TryGetConditionValue(target, _attribute.ConditionMemberName,
                    out object conditionValue))
            {
                return true;
            }

            return !ConditionMemberUtility.IsTruthy(conditionValue);
        }
    }
}
