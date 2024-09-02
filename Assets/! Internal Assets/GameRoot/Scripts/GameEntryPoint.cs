using R3;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Scenes;
using Zenject;
using Scenes.Gameplay.Params;
using Scenes.Gameplay;
using Scenes.MainMenu.Params;
using Scenes.MainMenu;

namespace GameRoot
{
    public class GameEntryPoint
    {
        private UIRootView _uiRoot;
        private Coroutines _coroutines;

        [Inject]
        public GameEntryPoint (UIRootView uiRoot)
        {
            _uiRoot = uiRoot;

            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);

            RunGame();
        }

        private void RunGame()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            switch (sceneName) 
            {
                case SceneNames.GAMEPLAY:
                    GameplayEnterParams enterParams = new GameplayEnterParams();
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

            yield return LoadScene(SceneNames.BOOT);

            yield return LoadScene(SceneNames.GAMEPLAY);

            GameplayEntryPoint sceneEntryPoint = Object.FindFirstObjectByType<GameplayEntryPoint>();
            sceneEntryPoint.Run(_uiRoot, enterParams).Subscribe(gameplayExitParams =>
            {
                _coroutines.StartCoroutine(LoadAndStartMainMenuScene(gameplayExitParams.MainMenuEnterParams));
            });

            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadAndStartMainMenuScene(MainMenuEnterParams enterParams = null)
        {
            _uiRoot.ShowLoadingScreen();

            yield return LoadScene(SceneNames.BOOT);

            yield return LoadScene(SceneNames.MAIN_MENU);

            MainMenuEntryPoint sceneEntryPoint = Object.FindFirstObjectByType<MainMenuEntryPoint>();
            sceneEntryPoint.Run(_uiRoot, enterParams).Subscribe(mainMenuExitParams =>
            {
                string targetSceneName = mainMenuExitParams.TargetSceneEnterParams.SceneName;
                if (targetSceneName == SceneNames.GAMEPLAY)
                {
                    GameplayEnterParams enterParams = mainMenuExitParams.TargetSceneEnterParams.As<GameplayEnterParams>();
                    _coroutines.StartCoroutine(LoadAndStartGameplayScene(enterParams));
                }
            });

            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
