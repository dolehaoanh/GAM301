using System;
using System.Linq;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.Core.Runtime.DependencyInjection
{
    public static class Dependencies
    {
        private static readonly DependencyResolver[] _resolvers;

        static Dependencies() => _resolvers = ReflectionFactory.CreateAllInstances<DependencyResolver>().ToArray();

        public static object Instantiate(Type type)
        {
            return GetRequiredResolver().Instantiate(type);
        }

        public static void BindInstance(Type type, string id, object instance)
        {
            GetRequiredResolver().BindInstance(type, id, instance);
        }

        public static bool TryResolve(Type type, out object instance)
        {
            return GetRequiredResolver().TryResolve(type, out instance);
        }

        public static bool TryResolveId(Type type, string id, out object instance)
        {
            return GetRequiredResolver().TryResolveId(type, id, out instance);
        }

        public static void UnbindId(Type type, string id)
        {
            GetRequiredResolver().UnbindId(type, id);
        }

        private static DependencyResolver GetRequiredResolver()
        {
            foreach (DependencyResolver resolver in _resolvers)
            {
                return resolver;
            }

            throw new InvalidOperationException("No dependency resolver is configured.");
        }
    }
}
