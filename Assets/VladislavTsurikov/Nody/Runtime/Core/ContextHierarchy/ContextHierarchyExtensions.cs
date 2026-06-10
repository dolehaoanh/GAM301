using System.Collections.Generic;

namespace VladislavTsurikov.Nody.Runtime.Core
{
    public static class ContextHierarchyExtensions
    {
        public static T GetContext<T>(this IReadOnlyList<object> hierarchy) where T : class
        {
            hierarchy.TryGetContext(out T result);
            return result;
        }

        public static bool TryGetContext<T>(this IReadOnlyList<object> hierarchy, out T result) where T : class
        {
            result = null;
            if (hierarchy == null)
            {
                return false;
            }

            for (int i = 0; i < hierarchy.Count; i++)
            {
                object entry = hierarchy[i];
                if (entry is T typed)
                {
                    result = typed;
                    return true;
                }
            }

            return false;
        }
    }
}
