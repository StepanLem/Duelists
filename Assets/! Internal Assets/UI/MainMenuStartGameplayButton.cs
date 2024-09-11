using System.Collections;
using UnityEngine;

public class MainMenuStartGameplayButton : MonoBehaviour
{
    public void StartGameplay()
    {
        StartCoroutine(StartGameplayRoutine());
    }

    private IEnumerator StartGameplayRoutine()
    {
        Fader.Instance.FadeIn();
        yield return new WaitUntil(() => !Fader.Instance.IsFading);

        LoadingScreenController.AddSceneToLoadOnNextLoadingScreen(SceneRegistry.GameplayScene);
        LoadingScreenController.AddSceneToUnloadOnNextLoadingScreen(SceneRegistry.MainMenuScene);
        LoadingScreenController.InvokeLoadingScreen(SceneRegistry.GameplayScene);
    }
}