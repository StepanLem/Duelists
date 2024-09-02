using GameRoot;
using R3;
using Scenes.Gameplay.Params;
using Scenes.MainMenu.Params;
using UnityEngine;

namespace Scenes.MainMenu
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIMainMenuRootBinder _sceneUIRootPrefab;

        public Observable<MainMenuExitParams> Run(UIRootView uiRoot, MainMenuEnterParams enterParams)
        {
            UIMainMenuRootBinder uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);

            Debug.Log(enterParams?.Result);

            Subject<Unit> _exitSignalSubj = new Subject<Unit>();
            uiScene.Bind(_exitSignalSubj);

            GameplayEnterParams gameplayEnterParams = new GameplayEnterParams();
            MainMenuExitParams mainMenuExitParams = new MainMenuExitParams(gameplayEnterParams);
            Observable<MainMenuExitParams>  exitToGameplaySceneSignal = _exitSignalSubj.Select(_ => mainMenuExitParams);

            return exitToGameplaySceneSignal;
        }
    }
}
