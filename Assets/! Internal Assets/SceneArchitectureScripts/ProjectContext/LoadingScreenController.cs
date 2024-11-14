using System;
using System.Collections;
using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadingScreenController
{
    private static SceneField _currentScene;

    public static event Action<float> OnLoadProgressChange;

    public static void LoadScene(SceneFieldReference newSceneRef)
    {
        SceneField newScene = newSceneRef.SceneField;
        LoadScene(newScene);
    }

    public static void LoadScene(SceneField newScene)
    {
        AsyncProcessor.StartRoutine(LoadSceneRoutine(newScene));
    }

    private static IEnumerator LoadSceneRoutine(SceneField newScene)
    {
        Time.timeScale = 0f;

        yield return AsyncProcessor.StartRoutine(LoadNewScene(SceneRegistry.LoadingScene));
        yield return new WaitUntil(() => LoadingScreen.Instance != null);
        yield return AsyncProcessor.StartRoutine(LoadingScreen.Instance.FadeIn(_currentScene == null));

        if (_currentScene != null)
        {
            yield return AsyncProcessor.StartRoutine(UnloadScene(_currentScene));
        }
        yield return AsyncProcessor.StartRoutine(LoadNewScene(newScene, true));

        yield return AsyncProcessor.StartRoutine(LoadingScreen.Instance.FadeOut());
        yield return AsyncProcessor.StartRoutine(UnloadScene(SceneRegistry.LoadingScene));

        Time.timeScale = 1f;

        _currentScene = newScene;
    }

    private static IEnumerator LoadNewScene(SceneField newScene, bool showLoadingProgress = false)
    {
        AsyncOperation loadNewScene = SceneManager.LoadSceneAsync(newScene.BuildIndex, LoadSceneMode.Additive);
        while (!loadNewScene.isDone)
        {
            if (showLoadingProgress)
            {
                OnLoadProgressChange?.Invoke(loadNewScene.progress);
            }
            yield return new WaitForEndOfFrame();
        }
        if (showLoadingProgress)
        {
            OnLoadProgressChange?.Invoke(loadNewScene.progress);
        }
    }

    private static IEnumerator UnloadScene(SceneField sceneToUnload)
    {
        AsyncOperation unloadCurrentSceneOperation = SceneManager.UnloadSceneAsync(sceneToUnload.BuildIndex);
        yield return new WaitUntil(() => unloadCurrentSceneOperation.isDone);
    }
}