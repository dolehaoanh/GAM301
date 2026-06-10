using System;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class FieldPresentationScope : IDisposable
    {
        protected FieldState State { get; }
        protected FieldStyle Style { get; }
        protected object FieldElement { get; }

        protected FieldPresentationScope(FieldState state, FieldStyle style, object fieldElement)
        {
            State = state;
            Style = style;
            FieldElement = fieldElement;
        }

        public abstract void Dispose();
    }
}
