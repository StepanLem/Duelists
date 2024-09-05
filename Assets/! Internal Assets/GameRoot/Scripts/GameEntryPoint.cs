using R3;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameEntryPoint : MonoBehaviour
{
    private async void Awake()
    {
        await RunGameAsync();
    }

    private async Task RunGameAsync()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != SceneName.BOOTSTRAP && sceneName != SceneName.LOADING)
        {
            SetupScene();
        }
        else
        {
            await LoadAndStartSceneAsync(SceneName.MAIN_MENU);
        }
    }

    private async Task LoadAndStartSceneAsync(string sceneName)
    {
        await LoadSceneAsync(SceneName.LOADING);
        await LoadSceneAsync(sceneName);

        SetupScene();
    }

    private void SetupScene()
    {
        SceneEntryPoint sceneEntryPoint = FindFirstObjectByType<SceneEntryPoint>();
        sceneEntryPoint.Run().Subscribe(async sceneName =>
        {
            await LoadAndStartSceneAsync(sceneName);
        });
    }

    private async Task LoadSceneAsync(string sceneName)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone)
        {
            await Task.Yield();
        }
    }
}

