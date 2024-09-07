using System;
using System.Collections;
using System.Collections.Generic;
using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadingScreenScene : MonoBehaviour
{
    public event Action<float> OnLoadProgressChange;

    //TODO: мб можно заменить статики на инициализацию через инжект. И/или всё время держать сцену загрузки в памяти.
    public static List<AsyncOperation> currentOperations = new();
    public static List<SceneField> ScenesToLoad = new();
    public static List<SceneField> ScenesToUnload = new();
    private static SceneField _sceneToSetActiveNext;

    public static void AddSceneToLoadOnNextLoadingScreen(SceneField scene)
    {
        ScenesToLoad.Add(scene);
    }

    public static void AddSceneToUnloadOnNextLoadingScreen(SceneField scene)
    {
        ScenesToUnload.Add(scene);
    }

    /// <summary>
    /// Загружает сцену "LoadingScreenScene". На ней происходят соотвествующие действия со ScenesToLoad и ScenesToUnload
    /// </summary>
    /// <param name="sceneToSetActiveNext">Сцена, которую надо сделать активной после LoadingScreenScene</param>
    public static void LoadAsync(SceneField sceneToSetActiveNext)
    {
        //TODO: сделать проверку на ошибки: sceneToSetActiveNext == null or sceneToSetActiveNext in ScenesToUnload

        _sceneToSetActiveNext = sceneToSetActiveNext;

        SceneManager.LoadSceneAsync(SceneRegistry.LoadingScene.BuildIndex, LoadSceneMode.Additive);
    }

    private void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(SceneRegistry.LoadingScene.BuildIndex));

        foreach (var scene in ScenesToUnload)
        {
            currentOperations.Add(SceneManager.UnloadSceneAsync(scene.BuildIndex));
        }
        ScenesToUnload.Clear();

        foreach (var scene in ScenesToLoad)
        {
            currentOperations.Add(SceneManager.LoadSceneAsync(scene.BuildIndex, LoadSceneMode.Additive));
        }
        ScenesToLoad.Clear();        

        StartCoroutine(nameof(CheckOperationsProgressRoutine));
    }

    private static readonly WaitForSeconds WaiterBetweenChecks = new(0.2f);

    private IEnumerator CheckOperationsProgressRoutine()
    {
        //TODO: запретить автоматически включаться сценам, когда их загрузка дошла до 0.9
        //А то они будут вклиниваться в сцену загрузки.

        var currentOperationsCount = currentOperations.Count;

        float resultProgress = 0;

        while (resultProgress != 1)//TODO: могу ошибаться, но во время запрета на автовключение максимальный уровень загрузки: 0.9 
        {
            float summaryProgress = 0;
            for (int i = 0; i < currentOperationsCount; i++)
            {
                summaryProgress += currentOperations[i].progress;
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

        currentOperations.Clear();

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(_sceneToSetActiveNext.BuildIndex));
        SceneManager.UnloadSceneAsync(SceneRegistry.LoadingScene.BuildIndex);
    }

}