using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VladislavTsurikov.DeepCopy.Runtime
{
    /// <inheritdoc />
    internal sealed class ReferenceEqualsComparer : IEqualityComparer<object>
    {
        /// <summary>
        ///     Gets an instance of this class.
        /// </summary>
        public static ReferenceEqualsComparer Instance { get; } = new();

        /// <inheritdoc />
        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        /// <inheritdoc />
        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return obj == null ? 0 : RuntimeHelpers.GetHashCode(obj);
        }
    }
}
