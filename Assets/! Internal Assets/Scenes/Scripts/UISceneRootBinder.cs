using R3;
using UnityEngine;

public class UISceneRootBinder : MonoBehaviour
{
    private Subject<string> _exitSceneSignalSubj;

    public void HandleGoToSceneButtonClick(string sceneName)
    {
        if (SceneName.IsCorrectName(sceneName))
        {
            _exitSceneSignalSubj?.OnNext(sceneName);
        }
        else
        {
            Debug.LogError($"WRONG SCENE NAME: {sceneName}");
        }
    }

    public void Bind(Subject<string> exitSceneSignalSubj)
    {
        _exitSceneSignalSubj = exitSceneSignalSubj;
    }
}
