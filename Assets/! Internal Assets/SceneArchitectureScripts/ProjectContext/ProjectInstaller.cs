using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<MatchManager>()
            .FromComponentInNewPrefabResource(nameof(MatchManager))
            .AsSingle();
    }
}
