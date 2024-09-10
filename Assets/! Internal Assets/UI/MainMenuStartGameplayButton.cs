using UnityEngine;

public class MainMenuStartGameplayButton : MonoBehaviour
{
    public void StartGameplay()
    {
        LoadingScreenController.AddSceneToLoadOnNextLoadingScreen(SceneRegistry.GameplayScene);
        LoadingScreenController.AddSceneToUnloadOnNextLoadingScreen(SceneRegistry.MainMenuScene);
        LoadingScreenController.InvokeLoadingScreen(SceneRegistry.GameplayScene);
    }
}