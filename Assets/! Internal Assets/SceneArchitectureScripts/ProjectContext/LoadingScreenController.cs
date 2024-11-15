using System;
using System.Collections;
using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadingScreenController
{
    private static SceneField _currentScene;
    private static AsyncOperation _loadNewSceneOperation;

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
        yield return AsyncProcessor.StartRoutine(LoadNewSceneRoutine(SceneRegistry.LoadingScene));
        yield return new WaitUntil(() => LoadingScreen.Instance != null);
        yield return AsyncProcessor.StartRoutine(LoadingScreen.Instance.FadeIn(_currentScene == null));

        if (_currentScene != null)
        {
            yield return AsyncProcessor.StartRoutine(UnloadSceneRoutine(_currentScene));
        }
        yield return AsyncProcessor.StartRoutine(LoadNewSceneRoutine(newScene, true));
        _loadNewSceneOperation.allowSceneActivation = false;

        yield return AsyncProcessor.StartRoutine(LoadingScreen.Instance.FadeOut());
        yield return AsyncProcessor.StartRoutine(UnloadSceneRoutine(SceneRegistry.LoadingScene));
        yield return new WaitForSecondsRealtime(1f);

        _currentScene = newScene;
    }

    private static IEnumerator LoadNewSceneRoutine(SceneField newScene, bool showLoadingProgress = false)
    {
        AsyncOperation loadNewSceneOperation = SceneManager.LoadSceneAsync(newScene.BuildIndex, LoadSceneMode.Additive);
        _loadNewSceneOperation = loadNewSceneOperation;
        if (showLoadingProgress)
        {
            yield return AsyncProcessor.StartRoutine(ShowLoadingProgressRoutine(loadNewSceneOperation));
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