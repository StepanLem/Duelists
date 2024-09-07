using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayExitGameButton : MonoBehaviour
{
    public void ExitGameplay()
    {
        LoadingScreenScene.AddSceneToLoadOnNextLoadingScreen(SceneRegistry.MainMenuScene);
        LoadingScreenScene.AddSceneToUnloadOnNextLoadingScreen(SceneRegistry.GameplayScene);
        LoadingScreenScene.LoadAsync(SceneRegistry.MainMenuScene);
    }
}
