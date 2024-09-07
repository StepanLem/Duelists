using System.Collections;
using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEntryPoint : MonoBehaviour
{
    public static int CurrentSceneBuildIndex;

    //Запускается при старте игры с любой из сцен. В не зависимости есть ли объект с этим скриптом на сцене.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BeforeGameStart()
    {
        SceneRegistry.InitializeFromScriptableObject();

#if UNITY_EDITOR && TEST
        int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneBuildIndex != SceneRegistry.PlayerVRScene.BuildIndex)
        {
            SceneManager.LoadScene(SceneRegistry.PlayerVRScene.BuildIndex);
        }
        if (currentSceneBuildIndex != SceneRegistry.Bootstrap.BuildIndex)
        {
            SceneManager.LoadScene(currentSceneBuildIndex, LoadSceneMode.Additive);
            CurrentSceneBuildIndex = currentSceneBuildIndex;
        }
#endif

        SceneManager.LoadScene(SceneRegistry.PlayerVRScene.BuildIndex);
        SceneManager.LoadScene(SceneRegistry.MainMenu.BuildIndex, LoadSceneMode.Additive);
        CurrentSceneBuildIndex = SceneRegistry.MainMenu.BuildIndex;
    }

    private static void LoadImportantScenes()
    {
        var isBootstrapLoaded = false;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);

            if (scene.buildIndex == SceneRegistry.Bootstrap.BuildIndex)
                isBootstrapLoaded = true;

            //TODO: то же самое со сценой игрока. В зависимости от того, подключён ли шлем.
        }

        if (!isBootstrapLoaded)
            SceneManager.LoadSceneAsync(SceneRegistry.Bootstrap.BuildIndex, LoadSceneMode.Additive);
    }
}

