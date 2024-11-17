using UnityEngine;
using UnityEngine.SceneManagement;

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
#endif
    }

    private void Awake()
    {
        if (FlatOrXRGameModeManager.TryActivateXRMode())
            SceneManager.LoadScene(SceneRegistry.PlayerVRScene.BuildIndex);
        else
            SceneManager.LoadScene(SceneRegistry.PlayerFlatScene.BuildIndex);

        LoadingScreenController.LoadScene(SceneRegistry.MainMenuScene);
    }
}

