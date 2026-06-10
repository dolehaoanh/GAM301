#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit.Decorators
{
    public sealed class HeaderDecoratorDrawerMatcher : DecoratorDrawerMatcher<UIToolkitDecoratorDrawer>
    {
        public override Type DrawerType => typeof(HeaderDecoratorDrawer);

        public override bool CanDraw(Attribute attribute)
        {
            return attribute is HeaderAttribute;
        }
    }

    public sealed class HeaderDecoratorDrawer : UIToolkitDecoratorDrawer
    {
        public override VisualElement CreateElement(FieldInfo field, object target)
        {
            if (Attribute is not HeaderAttribute headerAttribute)
            {
                return new VisualElement();
            }

            Label label = new(headerAttribute.header);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.marginTop = 8;
            label.style.marginBottom = 4;

            return label;
        }
    }
}
#endif
