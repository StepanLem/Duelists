using Assets.__Internal_Assets.UI.Architecture.GameRoot;
using UnityEngine;
using Zenject;

namespace Assets.__Internal_Assets.Architecture
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _rootView;

        public override void InstallBindings()
        {
            this.Container
                .Bind<Coroutines>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("[COROUTINES]")
                .AsSingle();

            this.Container.Bind<UIRootView>().FromComponentInNewPrefab(_rootView).AsSingle().NonLazy();
        }
    }
}
