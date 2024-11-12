using Zenject;

public class MainMenuSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<UISceneRootBinder>()
            .FromInstance(GetComponent<UISceneRootBinder>())
            .AsSingle();
    }
}