using Assets.__Internal_Assets.Architecture.Gameplay;
using Assets.__Internal_Assets.Architecture.MainMenu.View;
using Assets.__Internal_Assets.UI.Architecture.GameRoot;
using R3;
using System.Linq;
using UnityEngine;

namespace Assets.__Internal_Assets.Architecture.MainMenu
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIMainMenuRootBinder _sceneUIRootPrefab;

        public Observable<MainMenuExitParams> Run(UIRootView uiRoot, MainMenuEnterParams enterParams)
        {
            UIMainMenuRootBinder uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);

            Subject<Unit> exitSignalSubj = new Subject<Unit>();
            uiScene.Bind(exitSignalSubj);

            Debug.Log(enterParams?.Result);

            GameplayEnterParams gameplayEnterParams = new GameplayEnterParams(GameplayMode.Solo);
            MainMenuExitParams mainMenuExitParams = new MainMenuExitParams(gameplayEnterParams);
            Observable<MainMenuExitParams> exitToGameplaySceneSignal = exitSignalSubj.Select(_ => mainMenuExitParams);

            return exitToGameplaySceneSignal;
        }
    }
}
