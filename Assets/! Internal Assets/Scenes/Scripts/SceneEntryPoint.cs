using R3;
using UnityEngine;

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
