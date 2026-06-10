using System;

namespace Nody.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class GroupAttribute : Attribute
    {
        public readonly string Name;

        public GroupAttribute(string name) => Name = name;
    }
}
