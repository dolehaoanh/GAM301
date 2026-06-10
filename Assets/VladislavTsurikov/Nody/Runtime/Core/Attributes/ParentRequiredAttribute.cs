using System;

namespace VladislavTsurikov.Nody.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ParentRequiredAttribute : Attribute
    {
        public Type[] ParentTypes { get; }
        public ParentRequiredAttribute(params Type[] parentTypes) => ParentTypes = parentTypes ?? Array.Empty<Type>();
    }
}
