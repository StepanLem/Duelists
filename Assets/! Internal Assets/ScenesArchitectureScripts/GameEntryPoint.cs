using R3;
using System.Threading.Tasks;
using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEntryPoint : MonoBehaviour
{

    [SerializeField] private SceneField _playerVRScene;
    [SerializeField] private SceneField _menuScene;


    //Запускается при старте игры с любой из сцен. В не зависимости есть ли объект с этим скриптом на сцене.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BeforeGameStart()
    {
#if UNITY_EDITOR && TEST
        if(SceneManager.GetSceneByName(SceneNames.Bootstrap) != SceneManager.GetActiveScene()) //Если игру запускают с Bootstrap, то ожидается обычный запуск
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
            if (scene.name == SceneNames.Bootstrap)
                isBootstrapLoaded = true;

            //TODO: то же самое со сценой игрока. В зависимости от того, подключён ли шлем.

        }

        if (!isBootstrapLoaded)
            SceneManager.LoadSceneAsync(SceneNames.Bootstrap, LoadSceneMode.Additive);
    }
}

