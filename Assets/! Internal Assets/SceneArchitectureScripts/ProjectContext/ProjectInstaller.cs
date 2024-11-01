using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<MatchManager>()
            .FromNewComponentOnNewGameObject()
            .WithGameObjectName(nameof(MatchManager))
            .AsSingle();

        Container
            .Bind<DifficultySystem>()
            .FromComponentInNewPrefabResource(nameof(DifficultySystem))
            .AsSingle();
    }
}
