using GameRoot;
using R3;
using Scenes.Gameplay.Params;
using Scenes.MainMenu.Params;
using UnityEngine;

namespace Scenes.Gameplay
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;

        public Observable<GameplayExitParams> Run(UIRootView uiRoot, GameplayEnterParams enterParams)
        {
            UIGameplayRootBinder uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);

            Subject<Unit> exitSceneSignalSubj = new Subject<Unit>();
            uiScene.Bind(exitSceneSignalSubj);

            MainMenuEnterParams mainMenuEnterParams = new MainMenuEnterParams("Done");
            GameplayExitParams exitParams = new GameplayExitParams(mainMenuEnterParams);
            Observable<GameplayExitParams> exitToMainMenuSceneSignal = exitSceneSignalSubj.Select(_ => exitParams);

            return exitToMainMenuSceneSignal;
        }
    }
}
