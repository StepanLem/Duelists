using GameRoot;
using R3;
using UnityEngine;

namespace Scenes
{
    public class SceneEntryPoint : MonoBehaviour
    {
        [SerializeField] private UISceneRootBinder _sceneUI;

        public Observable<string> Run(UIRootView uiRoot)
        { 
            Subject<string> _exitSignalSubj = new Subject<string>();
            _sceneUI.Bind(_exitSignalSubj);

            return _exitSignalSubj;
        }
    }
}
