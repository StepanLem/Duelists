using GameRoot;
using R3;
using UnityEngine;

namespace Scenes
{
    public class SceneEntryPoint : MonoBehaviour
    {
        [SerializeField] private UISceneRootBinder _sceneUIRootPrefab;

        public Observable<string> Run(UIRootView uiRoot)
        {
            UISceneRootBinder uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);

            Subject<string> _exitSignalSubj = new Subject<string>();
            uiScene.Bind(_exitSignalSubj);

            return _exitSignalSubj;
        }
    }
}
