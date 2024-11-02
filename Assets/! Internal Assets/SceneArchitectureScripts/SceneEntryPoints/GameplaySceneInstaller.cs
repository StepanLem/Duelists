using Zenject;

public class GameplaySceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<Round>()
            .FromNewComponentOnNewGameObject()
            .WithGameObjectName(nameof(Round))
            .AsSingle();
    }
}
