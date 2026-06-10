#if COMPONENT_STACK_ZENJECT
using UnityEngine.SceneManagement;
using VladislavTsurikov.Nody.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.Nody.Runtime
{
    public sealed class ZenjectNodeInjectorRegistrar : NodeInjectorRegistrar
    {
        public override void Inject(Element node)
        {
            ProjectContext projectContext = ProjectContext.Instance;
            if (projectContext == null)
            {
                return;
            }

            DiContainer container = null;
            if (projectContext.Container != null)
            {
                SceneContextRegistry registry = projectContext.Container.Resolve<SceneContextRegistry>();
                if (registry != null)
                {
                    container = registry.TryGetContainerForScene(SceneManager.GetActiveScene());
                }
            }

            DiContainer projectContainer = projectContext.Container;
            if (container == null || ReferenceEquals(container, projectContainer))
            {
                projectContainer?.Inject(node);
                return;
            }

            try
            {
                container.Inject(node);
            }
            catch
            {
                projectContainer?.Inject(node);
            }
        }
    }
}
#endif
