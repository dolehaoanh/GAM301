#if UI_SYSTEM_ZENJECT
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace VladislavTsurikov.Core.Runtime.DependencyInjection
{
    public sealed class UIDocumentInstaller : MonoInstaller
    {
        [SerializeField]
        private UIDocument _document;

        private void OnDestroy()
        {
            ProjectContext.Instance.Container.Unbind<UIDocument>();
        }

        public override void InstallBindings()
        {
            if (_document == null)
            {
                throw new InvalidOperationException("UIDocument is not assigned.");
            }

            ProjectContext.Instance.Container.Bind<UIDocument>().FromInstance(_document).AsSingle();
        }
    }
}
#endif
