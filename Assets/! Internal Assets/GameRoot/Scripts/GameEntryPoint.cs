using R3;
using System.Threading.Tasks;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameEntryPoint : MonoBehaviour
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnGameStart()
    {
#if UNITY_EDITOR
        LoadImportantScenes();
#endif
    }

    private static void LoadImportantScenes()
    {
        var isBootstrapLoaded = false;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);

            //�� ����� ����� ������� ������� ����� ScriptableObject
            //�� ����� ������� � ���� ������������ � ������� sceneField.
            //����� �� ��������� ��������� ������� + �������� � ���������� ���������� �� �����.
            //������ � ��� ��������� ��������� �������� ������ �� ������ �������.
            //TODO: ���� ����������� � ������.
            if (scene.name == SceneName.Bootstrap)
                isBootstrapLoaded = true;

            //TODO: �� �� ����� �� ������ ������. � ����������� �� ����, ��������� �� ����.

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
