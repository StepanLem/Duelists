using R3;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameEntryPoint : MonoBehaviour
{
    private UIRootView _uiRoot;

    [Inject]
    public void Construct(UIRootView uiRoot)
    {
        _uiRoot = uiRoot;
    }

    private async void Awake()
    {
        await RunGameAsync();
    }

    private async Task RunGameAsync()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != SceneName.BOOTSTRAP)
        {
            await LoadAndStartSceneAsync(sceneName);
        }
        else
        {
            await LoadAndStartSceneAsync(SceneName.MAIN_MENU);
        }
    }

    private async Task LoadAndStartSceneAsync(string sceneName)
    {
        _uiRoot.ShowLoadingScreen();

        await LoadSceneAsync(SceneName.BOOTSTRAP);
        await LoadSceneAsync(sceneName);

        SceneEntryPoint sceneEntryPoint = FindFirstObjectByType<SceneEntryPoint>();
        sceneEntryPoint.Run(_uiRoot).Subscribe(async sceneName =>
        {
            await LoadAndStartSceneAsync(sceneName);
        });

        _uiRoot.HideLoadingScreen();
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

