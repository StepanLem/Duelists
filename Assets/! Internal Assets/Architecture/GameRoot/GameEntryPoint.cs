using Assets.__Internal_Assets.Architecture;
using Assets.__Internal_Assets.Architecture.Gameplay;
using Assets.__Internal_Assets.Architecture.MainMenu;
using Assets.__Internal_Assets.UI.Architecture.GameRoot;
using R3;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameEntryPoint
{
    private static GameEntryPoint _instance;

    private Coroutines _coroutines;
    private UIRootView _uiRoot;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void StartGame()
    {
        _instance = new GameEntryPoint();
        _instance.RunGame();
    }

    public GameEntryPoint()
    {
        _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
        Object.DontDestroyOnLoad(_coroutines);

        UIRootView prefabUIRoot = Resources.Load<UIRootView>("UIRoot");
        _uiRoot = Object.Instantiate(prefabUIRoot);
        Object.DontDestroyOnLoad(_uiRoot.gameObject);
    }

    private void RunGame()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName) 
        {
            case Scenes.GAMEPLAY:
                GameplayEnterParams enterParams = new GameplayEnterParams(GameplayMode.Solo);
                _coroutines.StartCoroutine(LoadAndStartGameplayScene(enterParams));
                break;
            default:
                _coroutines.StartCoroutine(LoadAndStartMainMenuScene());
                break;
        }
    }

    private IEnumerator LoadAndStartGameplayScene(GameplayEnterParams enterParams)
    {
        _uiRoot.ShowLoadingScreen();

        yield return LoadScene(Scenes.BOOT);

        yield return LoadScene(Scenes.GAMEPLAY);

        yield return new WaitForSeconds(1f);

        GameplayEntryPoint sceneEntryPoint = Object.FindFirstObjectByType<GameplayEntryPoint>();
        sceneEntryPoint.Construct(_uiRoot, enterParams).Subscribe(gameplayExitParams =>
        {
            _coroutines.StartCoroutine(LoadAndStartMainMenuScene(gameplayExitParams.MainMenuEnterParams));
        });

        _uiRoot.HideLoadingScreen();
    }

    private IEnumerator LoadAndStartMainMenuScene(MainMenuEnterParams enterParams = null)
    {
        _uiRoot.ShowLoadingScreen();

        yield return LoadScene(Scenes.BOOT);

        yield return LoadScene(Scenes.MAIN_MENU);

        yield return new WaitForSeconds(1f);

        MainMenuEntryPoint sceneEntryPoint = Object.FindFirstObjectByType<MainMenuEntryPoint>();
        sceneEntryPoint.Run(_uiRoot, enterParams).Subscribe(mainMenuExitParams =>
        {
            string targetSceneName = mainMenuExitParams.TargetSceneEnterParams.SceneName;

            if (targetSceneName == Scenes.GAMEPLAY)
            {
                _coroutines.StartCoroutine(LoadAndStartGameplayScene(mainMenuExitParams.TargetSceneEnterParams.As<GameplayEnterParams>()));
            }
        });

        _uiRoot.HideLoadingScreen();
    }

    private IEnumerator LoadScene(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
    }
}
