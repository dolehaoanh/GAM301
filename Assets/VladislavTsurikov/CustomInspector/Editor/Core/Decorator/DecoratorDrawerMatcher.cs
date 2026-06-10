using System;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class DecoratorDrawerMatcher<TDrawer> where TDrawer : DecoratorDrawer
    {
        public abstract Type DrawerType { get; }
        public abstract bool CanDraw(Attribute attribute);
    }
}
