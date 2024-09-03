using R3;
using System.Threading.Tasks;
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

        private async void Awake()
        {
            await RunGameAsync();
        }

        private async Task RunGameAsync()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            switch (sceneName)
            {
                case SceneNames.GAMEPLAY:
                    await LoadAndStartGameplaySceneAsync();
                    break;
                default:
                    await LoadAndStartMainMenuSceneAsync();
                    break;
            }
        }

        private async Task LoadAndStartGameplaySceneAsync()
        {
            _uiRoot.ShowLoadingScreen();

            await LoadSceneAsync(SceneNames.BOOT);
            await LoadSceneAsync(SceneNames.GAMEPLAY);

            GameplayEntryPoint sceneEntryPoint = FindFirstObjectByType<GameplayEntryPoint>();
            sceneEntryPoint.Run(_uiRoot).Subscribe(async _ =>
            {
                await LoadAndStartMainMenuSceneAsync();
            });

            _uiRoot.HideLoadingScreen();
        }

        private async Task LoadAndStartMainMenuSceneAsync()
        {
            _uiRoot.ShowLoadingScreen();

            await LoadSceneAsync(SceneNames.BOOT);
            await LoadSceneAsync(SceneNames.MAIN_MENU);

            MainMenuEntryPoint sceneEntryPoint = FindFirstObjectByType<MainMenuEntryPoint>();
            sceneEntryPoint.Run(_uiRoot).Subscribe(async _ =>
            {
                await LoadAndStartGameplaySceneAsync();
            });

            _uiRoot.HideLoadingScreen();
        }

        private async Task LoadSceneAsync(string sceneName)
        {
            var load = SceneManager.LoadSceneAsync(sceneName);
            while (!load.isDone)
            {
                await Task.Yield();
            }
        }
    }
}
