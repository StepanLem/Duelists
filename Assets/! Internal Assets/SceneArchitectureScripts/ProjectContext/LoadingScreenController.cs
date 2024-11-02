using System;
using System.Collections;
using System.Collections.Generic;
using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadingScreenController
{
    private static readonly List<AsyncOperation> _currentOperations = new();
    private static readonly List<SceneField> _scenesToLoad = new();
    private static readonly List<SceneField> _scenesToUnload = new();
    private static SceneField _sceneToSetActiveNext;

    public static event Action<float> OnLoadProgressChange;

    public static void AddSceneToLoadOnNextLoadingScreen(SceneField scene)
    {
        _scenesToLoad.Add(scene);
    }

    public static void AddSceneToUnloadOnNextLoadingScreen(int buildIndex)
    {
        AddSceneToUnloadOnNextLoadingScreen(SceneRegistry.GetSceneFieldByBuildIndex(buildIndex));
    }

    public static void AddSceneToUnloadOnNextLoadingScreen(SceneField scene)
    {
        _scenesToUnload.Add(scene);
    }

    /// <summary>
    /// Загружает сцену "LoadingScreenScene". 
    /// Во время её существования происходят соотвествующие действия со ScenesToLoad и ScenesToUnload
    /// </summary>
    /// <param name="sceneToSetActiveNext">Сцена, которую надо сделать активной после LoadingScreenScene</param>
    public static void InvokeLoadingScreen(SceneField sceneToSetActiveNext)
    {
        Time.timeScale = 0f;
        if (sceneToSetActiveNext == null || _scenesToUnload.Contains(sceneToSetActiveNext))
        {
            Debug.LogError("Ошибка: сцена не может стать активной.");
            return;
        }
        _sceneToSetActiveNext = sceneToSetActiveNext;

        SceneManager.sceneLoaded += OnLoadingSceneLoaded;

        SceneManager.LoadSceneAsync(SceneRegistry.LoadingScene.BuildIndex, LoadSceneMode.Additive);
        Time.timeScale = 1f;
    }

    private static void OnLoadingSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Scene loadingScene = SceneManager.GetSceneByBuildIndex(SceneRegistry.LoadingScene.BuildIndex);

        if (scene != loadingScene)
            return;

        SceneManager.sceneLoaded -= OnLoadingSceneLoaded;

        SceneManager.SetActiveScene(loadingScene);

        UnloadScenesToUndloadAsync();
        LoadScenesToLoadAsync();

        AsyncProcessor.StartRoutine(CheckOperationsProgressRoutine());
    }

    private static readonly WaitForSeconds WaiterBetweenChecks = new(0.2f);
    private static IEnumerator CheckOperationsProgressRoutine()
    {
        //TODO: запретить автоматически включаться сценам, когда их загрузка дошла до 0.9
        //А то они будут вклиниваться в сцену загрузки.

        var currentOperationsCount = _currentOperations.Count;

        float resultProgress = 0;

        while (resultProgress != 1)//TODO: могу ошибаться, но во время запрета на автовключение максимальный уровень загрузки: 0.9 
        {
            float summaryProgress = 0;
            for (int i = 0; i < currentOperationsCount; i++)
            {
                summaryProgress += _currentOperations[i].progress;
            }

            resultProgress = summaryProgress / currentOperationsCount;
            OnLoadProgressChange?.Invoke(resultProgress);

#if UNITY_EDITOR
            if (resultProgress > 0.5f)
                yield return new WaitForSeconds(1f);
#endif

            yield return WaiterBetweenChecks;
        }

        //TODO: когда всё полностью загружено: разрешать показываться сценам;(мб после разрешения надо прождать ещё один кадр)

        _currentOperations.Clear();

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(_sceneToSetActiveNext.BuildIndex));
        SceneManager.UnloadSceneAsync(SceneRegistry.LoadingScene.BuildIndex);
    }

    public static void UnloadScenesToUndloadAsync()
    {
        foreach (var scene in _scenesToUnload)
        {
            _currentOperations.Add(SceneManager.UnloadSceneAsync(scene.BuildIndex));
        }
        _scenesToUnload.Clear();
    }

    public static void LoadScenesToLoadAsync()
    {
        foreach (var scene in _scenesToLoad)
        {
            _currentOperations.Add(SceneManager.LoadSceneAsync(scene.BuildIndex, LoadSceneMode.Additive));
        }
        _scenesToLoad.Clear();
    }
}