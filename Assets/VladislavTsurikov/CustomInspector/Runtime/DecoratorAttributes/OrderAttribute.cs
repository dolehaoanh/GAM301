using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class OrderAttribute : Attribute
    {
        public int Order { get; }
        public OrderAttribute(int order) => Order = order;
    }
}
