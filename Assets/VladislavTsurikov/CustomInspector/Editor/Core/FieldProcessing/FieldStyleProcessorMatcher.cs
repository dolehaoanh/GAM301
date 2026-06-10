using System;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class FieldStyleProcessorMatcher
    {
        public abstract Type ProcessorType { get; }
        public abstract bool CanProcess(Attribute attribute);
    }
}
