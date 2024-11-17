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
        yield return LoadNewSceneRoutine(SceneRegistry.LoadingScene);
        yield return new WaitUntil(() => LoadingScreen.Instance != null);
        yield return LoadingScreen.Instance.FadeIn(_currentScene == null);

        if (_currentScene != null)
        {
            yield return UnloadSceneRoutine(_currentScene);
        }
        yield return LoadNewSceneRoutine(newScene, true);

        yield return AsyncProcessor.StartRoutine(LoadingScreen.Instance.FadeOut());
        yield return UnloadSceneRoutine(SceneRegistry.LoadingScene);
        Time.timeScale = 1.0f;

        _currentScene = newScene;
    }

    private static IEnumerator LoadNewSceneRoutine(SceneField newScene, bool showLoadingProgress = false)
    {
        AsyncOperation loadNewSceneOperation = SceneManager.LoadSceneAsync(newScene.BuildIndex, LoadSceneMode.Additive);
        if (showLoadingProgress)
        {
            yield return ShowLoadingProgressRoutine(loadNewSceneOperation);
        }
        else
        {
            yield return new WaitUntil(() => loadNewSceneOperation.isDone);
        }
    }

    private static IEnumerator ShowLoadingProgressRoutine(AsyncOperation loadNewSceneOperation)
    {
        while (!loadNewSceneOperation.isDone)
        {
            OnLoadProgressChange?.Invoke(loadNewSceneOperation.progress);
            yield return new WaitForEndOfFrame();
        }
        OnLoadProgressChange?.Invoke(loadNewSceneOperation.progress);
    }

    private static IEnumerator UnloadSceneRoutine(SceneField sceneToUnload)
    {
        AsyncOperation unloadCurrentSceneOperation = SceneManager.UnloadSceneAsync(sceneToUnload.BuildIndex);
        yield return new WaitUntil(() => unloadCurrentSceneOperation.isDone);
    }
}