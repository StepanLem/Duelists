using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayExitGameButton : MonoBehaviour
{
    public void ExitGameplay()
    {
        LoadingScreenController.AddSceneToLoadOnNextLoadingScreen(SceneRegistry.MainMenuScene);
        LoadingScreenController.AddSceneToUnloadOnNextLoadingScreen(SceneRegistry.GameplayScene);
        LoadingScreenController.InvokeLoadingScreen(SceneRegistry.MainMenuScene);
    }
}
