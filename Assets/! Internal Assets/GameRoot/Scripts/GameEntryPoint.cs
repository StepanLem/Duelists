using R3;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scenes;
using Zenject;
using Scenes.Gameplay;
using Scenes.MainMenu;

namespace GameRoot
{
    public class GameEntryPoint : MonoBehaviour
    {
        private UIRootView _uiRoot;

        [Inject]
        public void Construct(UIRootView uiRoot)
        {
            _uiRoot = uiRoot;
        }

        private void Awake()
        {
            RunGame();
        }

        private void RunGame()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            switch (sceneName) 
            {
                case SceneNames.GAMEPLAY:
                    StartCoroutine(LoadAndStartGameplayScene());
                    break;
                default:
                    StartCoroutine(LoadAndStartMainMenuScene());
                    break;
            }
        }

        private IEnumerator LoadAndStartGameplayScene()
        {
            _uiRoot.ShowLoadingScreen();

            yield return LoadScene(SceneNames.BOOT);

            yield return LoadScene(SceneNames.GAMEPLAY);

            GameplayEntryPoint sceneEntryPoint = FindFirstObjectByType<GameplayEntryPoint>();
            sceneEntryPoint.Run(_uiRoot).Subscribe(_ =>
            {
                StartCoroutine(LoadAndStartMainMenuScene());
            });

            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadAndStartMainMenuScene()
        {
            _uiRoot.ShowLoadingScreen();

            yield return LoadScene(SceneNames.BOOT);

            yield return LoadScene(SceneNames.MAIN_MENU);

            MainMenuEntryPoint sceneEntryPoint = FindFirstObjectByType<MainMenuEntryPoint>();
            sceneEntryPoint.Run(_uiRoot).Subscribe(_ =>
            {
                StartCoroutine(LoadAndStartGameplayScene());
            });

            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
