using R3;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEntryPoint : MonoBehaviour
{
    //Запускается при старте игры с любой из сцен. В не зависимости есть ли объект с этим скриптом на сцене.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnGameStart()
    {
#if UNITY_EDITOR && TEST
        if(SceneManager.GetSceneByName(SceneName.Bootstrap) != SceneManager.GetActiveScene()) //Если игру запускают с Bootstrap, то ожидается обычный запуск
            LoadImportantScenes();
#endif
    }

    private static void LoadImportantScenes()
    {
        var isBootstrapLoaded = false;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);

            //мб можно более красиво сделать через ScriptableObject
            //со всеми сценами в него перенесёнными с помощью sceneField.
            //Чтобы не обновлять константы вручную + проблемы с возможными опечатками не будет.
            //Вопрос в том насколько некрасиво получать данные из SO.
            //TODO: Надо попробовать и узнать.
            if (scene.name == SceneName.Bootstrap)
                isBootstrapLoaded = true;

            //TODO: то же самое со сценой игрока. В зависимости от того, подключён ли шлем.

        }

        if (!isBootstrapLoaded)
            SceneManager.LoadSceneAsync(SceneName.Bootstrap, LoadSceneMode.Additive);
    }


    /*private async void Awake()
    {
        await RunGameAsync();
    }*/

    private async Task RunGameAsync()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != SceneName.Bootstrap && sceneName != SceneName.Loading)
        {
            SetupScene();
        }
        else
        {
            await LoadAndStartSceneAsync(SceneName.MainMenu);
        }
    }

    private async Task LoadAndStartSceneAsync(string sceneName)
    {
        await LoadSceneAsync(SceneName.Loading);
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

