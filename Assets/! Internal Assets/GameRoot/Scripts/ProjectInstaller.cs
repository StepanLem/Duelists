using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        this.Container
            .Bind<GameEntryPoint>()
            .FromComponentInNewPrefabResource("GameEntryPoint")
            .AsSingle()
            .NonLazy();
    }
}
