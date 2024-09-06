using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEntryPoint : MonoBehaviour
{
    //Запускается при старте игры с любой из сцен. В не зависимости есть ли объект с этим скриптом на сцене.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BeforeGameStart()
    {
        SceneRegistry.InitializeFromScriptableObject();

#if UNITY_EDITOR && TEST
        if (SceneRegistry.Bootstrap.BuildIndex == SceneManager.GetActiveScene().buildIndex) //Если игру запускают с Bootstrap, то ожидается обычный запуск
            return;

        
        LoadImportantScenes();
#endif
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

