using Assets.__Internal_Assets.Architecture.Gameplay.View;
using Assets.__Internal_Assets.Architecture.MainMenu;
using Assets.__Internal_Assets.UI.Architecture.GameRoot;
using R3;
using UnityEngine;
using Zenject;

namespace Assets.__Internal_Assets.Architecture.Gameplay
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;

        public Observable<GameplayExitParams> Construct(UIRootView uiRoot, GameplayEnterParams enterParams)
        {
            UIGameplayRootBinder uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);

            Subject<Unit> exitSceneSignalSubj = new Subject<Unit>();
            uiScene.Bind(exitSceneSignalSubj);

            Debug.Log(enterParams.Mode);

            MainMenuEnterParams mainMenuEnterParams = new MainMenuEnterParams("Done");
            GameplayExitParams exitParams = new GameplayExitParams(mainMenuEnterParams);
            Observable<GameplayExitParams> exitToMainMenuSceneSignal = exitSceneSignalSubj.Select(_ => exitParams);

            return exitToMainMenuSceneSignal;
        }
    }
}
