using System;
using System.Collections.Generic;
using System.Linq;
using Nody.Runtime.Core;
using VladislavTsurikov.AttributeUtility.Runtime;

namespace VladislavTsurikov.Nody.Runtime.Core
{
    public sealed class AllowedGroup
    {
        public string[] AllowedGroupAttributesData { get; private set; }

        public void Set(string[] allowedGroupAttributes)
        {
            AllowedGroupAttributesData = allowedGroupAttributes;
        }

        public bool IsGroupAllowed(Type settingsType)
        {
            IEnumerable<GroupAttribute> groupAttributes = settingsType.GetAttributes<GroupAttribute>();

            if (AllowedGroupAttributesData == null || AllowedGroupAttributesData.Length == 0)
            {
                if (groupAttributes.Any())
                {
                    return false;
                }

                return true;
            }

            foreach (GroupAttribute group in groupAttributes)
            {
                foreach (string t in AllowedGroupAttributesData)
                {
                    if (string.Equals(group.Name, t, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
