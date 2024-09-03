using GameRoot;
using R3;
using UnityEngine;

namespace Scenes.Gameplay
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;

        public Observable<Unit> Run(UIRootView uiRoot)
        {
            UIGameplayRootBinder uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);

            Subject<Unit> exitSceneSignalSubj = new Subject<Unit>();
            uiScene.Bind(exitSceneSignalSubj);

            return exitSceneSignalSubj;
        }
    }
}
