using Zenject;

public class GameplaySceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<Round>()
            .FromInstance(GetComponent<Round>())
            .AsSingle();
    }
}
