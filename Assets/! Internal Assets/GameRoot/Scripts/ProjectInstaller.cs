using Zenject;

namespace GameRoot
{
    public class ProjectContext : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container
                .Bind<UIRootView>()
                .FromComponentInNewPrefabResource("UIRoot")
                .AsSingle();
            this.Container
                .Bind<GameEntryPoint>()
                .FromComponentInNewPrefabResource("GameEntryPoint")
                .AsSingle()
                .NonLazy();
        }
    }
}
