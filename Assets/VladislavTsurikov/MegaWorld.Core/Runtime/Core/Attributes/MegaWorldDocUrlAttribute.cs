using System;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MegaWorldDocUrlAttribute : DocUrlAttribute
    {
        public MegaWorldDocUrlAttribute(string path)
            : base(DocumentationDomain.Url + MegaWorldPath.DocPrefix + "/" + path)
        {
        }
    }
}
