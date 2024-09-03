using GameRoot;
using R3;
using UnityEngine;

namespace Scenes
{
    public class SceneEntryPoint : MonoBehaviour
    {
        [SerializeField] private UISceneRootBinder _sceneUIRootPrefab;

        public Observable<SceneName> Run(UIRootView uiRoot)
        {
            UISceneRootBinder uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);

            Subject<SceneName> _exitSignalSubj = new Subject<SceneName>();
            uiScene.Bind(_exitSignalSubj);

            return _exitSignalSubj;
        }
    }
}
