using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ButtonAttribute : Attribute
    {
        public ButtonSize ButtonSize { get; }
        public string Name { get; }

        public ButtonAttribute(ButtonSize buttonSize = ButtonSize.Medium, string name = null)
        {
            ButtonSize = buttonSize;
            Name = name;
        }

        public ButtonAttribute(string name)
        {
            ButtonSize = ButtonSize.Medium;
            Name = name;
        }
    }

    public enum ButtonSize
    {
        Small,
        Medium,
        Large
    }
}
