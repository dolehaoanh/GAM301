using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.Nody.Runtime.Core
{
    public static class NodeInjectionUtility
    {
        private static readonly List<NodeInjectorRegistrar> _registrars;

        static NodeInjectionUtility() =>
            _registrars = new List<NodeInjectorRegistrar>(
                ReflectionFactory.CreateAllInstances<NodeInjectorRegistrar>());

        public static void Inject(Element node)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            foreach (NodeInjectorRegistrar t in _registrars)
            {
                t.Inject(node);
            }
        }
    }
}
