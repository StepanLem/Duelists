using Zenject;

public class GameplaySceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<UISceneRootBinder>()
            .FromInstance(GetComponent<UISceneRootBinder>())
            .AsSingle();
        Container
            .Bind<Match>()
            .FromInstance(GetComponent<Match>())
            .AsSingle();
    }
}
