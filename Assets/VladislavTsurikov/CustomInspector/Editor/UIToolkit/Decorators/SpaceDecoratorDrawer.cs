#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit.Decorators
{
    public sealed class SpaceDecoratorDrawerMatcher : DecoratorDrawerMatcher<UIToolkitDecoratorDrawer>
    {
        public override Type DrawerType => typeof(SpaceDecoratorDrawer);

        public override bool CanDraw(Attribute attribute)
        {
            return attribute is SpaceAttribute;
        }
    }

    public sealed class SpaceDecoratorDrawer : UIToolkitDecoratorDrawer
    {
        public override VisualElement CreateElement(FieldInfo field, object target)
        {
            float height = 8f;

            if (Attribute is SpaceAttribute spaceAttribute)
            {
                height = spaceAttribute.height;
            }

            VisualElement space = new();
            space.style.height = height;

            return space;
        }
    }
}
#endif
