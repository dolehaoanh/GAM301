using System;
using System.Linq;
using UnityEngine;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public static class TypeExtensions
    {
        public static bool MatchesAnyFilter(this Type type, Func<FilterAttribute, bool> predicate)
        {
            FilterAttribute[] filters = type
                .GetCustomAttributes(typeof(FilterAttribute), true)
                .Cast<FilterAttribute>()
                .ToArray();

            if (filters.Length == 0)
            {
                Debug.LogError(
                    $"[UIPresenterManager] Type `{type.FullName}` does not have any FilterAttribute. This is required.");
                return false;
            }

            return filters.Any(predicate);
        }
    }
}
