#if UI_SYSTEM_ZENJECT
using System;
using Zenject;

namespace VladislavTsurikov.Core.Runtime.DependencyInjection
{
    public sealed class ZenjectUIDependencyResolver : DependencyResolver
    {
        public override object Instantiate(Type type)
        {
            return ProjectContext.Instance.Container.Instantiate(type);
        }

        public override void BindInstance(Type type, string id, object instance)
        {
            ProjectContext.Instance.Container.Bind(type).WithId(id).FromInstance(instance).AsCached();
        }

        public override bool TryResolve(Type type, out object instance)
        {
            instance = ProjectContext.Instance.Container.TryResolve(type);
            return instance != null;
        }

        public override bool TryResolveId(Type type, string id, out object instance)
        {
            instance = ProjectContext.Instance.Container.TryResolveId(type, id);
            return instance != null;
        }

        public override void UnbindId(Type type, string id)
        {
            ProjectContext.Instance.Container.UnbindId(type, id);
        }
    }
}
#endif
