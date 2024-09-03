using GameRoot;
using R3;
using UnityEngine;

namespace Scenes.MainMenu
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIMainMenuRootBinder _sceneUIRootPrefab;

        public Observable<Unit> Run(UIRootView uiRoot)
        {
            UIMainMenuRootBinder uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);

            Subject<Unit> _exitSignalSubj = new Subject<Unit>();
            uiScene.Bind(_exitSignalSubj);

            return _exitSignalSubj;
        }
    }
}
