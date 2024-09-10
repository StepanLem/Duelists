using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayExitGameButton : MonoBehaviour
{
    public void ExitGameplay()
    {
        StartCoroutine(ExitGameplayRoutine());
    }

    private IEnumerator ExitGameplayRoutine()
    {
        Fader.Instance.FadeOut();
        while (Fader.Instance.IsFading)
        {
            yield return new WaitUntil(() => !Fader.Instance.IsFading);
        }

        LoadingScreenController.AddSceneToLoadOnNextLoadingScreen(SceneRegistry.MainMenuScene);
        LoadingScreenController.AddSceneToUnloadOnNextLoadingScreen(SceneRegistry.GameplayScene);
        LoadingScreenController.InvokeLoadingScreen(SceneRegistry.MainMenuScene);
    }
}
