using System;

namespace VladislavTsurikov.Nody.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DocUrlAttribute : Attribute
    {
        public virtual string Url { get; }
        public DocUrlAttribute(string url) => Url = url;
    }
}
