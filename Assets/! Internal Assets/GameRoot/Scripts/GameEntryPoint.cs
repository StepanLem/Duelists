using R3;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scenes;
using Zenject;
using System;

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
            string sceneNameStr = SceneManager.GetActiveScene().name;
            if (Enum.TryParse(sceneNameStr, out SceneName sceneName)) 
            {
                if (sceneName != SceneName.Boot)
                {
                    await LoadAndStartSceneAsync(sceneName);
                }
                else
                {
                    await LoadAndStartSceneAsync(SceneName.MainMenu);
                }
            }
            
        }

        private async Task LoadAndStartSceneAsync(SceneName sceneName)
        {
            _uiRoot.ShowLoadingScreen();

            await LoadSceneAsync(SceneName.Boot);
            await LoadSceneAsync(sceneName);

            SceneEntryPoint sceneEntryPoint = FindFirstObjectByType<SceneEntryPoint>();
            sceneEntryPoint.Run(_uiRoot).Subscribe(async sceneName =>
            {
                await LoadAndStartSceneAsync(sceneName);
            });

            _uiRoot.HideLoadingScreen();
        }

        private async Task LoadSceneAsync(SceneName sceneName)
        {
            var load = SceneManager.LoadSceneAsync(sceneName.ToString());
            while (!load.isDone)
            {
                await Task.Yield();
            }
        }
    }
}
