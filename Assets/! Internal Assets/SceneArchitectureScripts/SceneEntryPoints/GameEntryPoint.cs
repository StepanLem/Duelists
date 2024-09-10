using System.Collections;
using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEntryPoint : MonoBehaviour
{
    /// <summary>
    /// Запускается при старте игры с любой из сцен. Вне зависимости есть ли объект с этим скриптом на сцене.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BeforeGameStart()
    {
        SceneRegistry.InitializeFromScriptableObject();

#if UNITY_EDITOR && TEST
        int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

        //если игра запускается с Bootstrap, то запуск должен идти как ожидается.
        if (currentSceneBuildIndex == SceneRegistry.BootstrapScene.BuildIndex)
            return;

        //TODO: Загрузка VR/nonVR сцены с игроком в зависимости от подключённого шлема.
        SceneField currentScene = SceneRegistry.FindSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex);
        LoadingScreenController.AddSceneToLoadOnNextLoadingScreen(currentScene);

        SceneManager.LoadScene(SceneRegistry.PlayerVRScene.BuildIndex);
        LoadingScreenController.InvokeLoadingScreen(currentScene);
#endif


    }


    private void Awake()
    {
        SceneManager.LoadScene(SceneRegistry.PlayerVRScene.BuildIndex);

        LoadingScreenController.AddSceneToLoadOnNextLoadingScreen(SceneRegistry.MainMenuScene);
        LoadingScreenController.InvokeLoadingScreen(SceneRegistry.MainMenuScene);
    }
}

