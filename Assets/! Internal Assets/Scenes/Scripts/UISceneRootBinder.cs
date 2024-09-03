using R3;
using UnityEngine;

namespace Scenes
{
    public class UISceneRootBinder : MonoBehaviour
    {
        [SerializeField] private SceneName _exitSceneName;

        private Subject<SceneName> _exitSceneSignalSubj;

        public void HandleGoToSceneButtonClick()
        {
            _exitSceneSignalSubj?.OnNext(_exitSceneName);
        }

        public void HandleGoToSceneButtonClick(SceneName sceneName)
        {
            _exitSceneSignalSubj?.OnNext(sceneName);
        }

        public void Bind(Subject<SceneName> exitSceneSignalSubj)
        {
            _exitSceneSignalSubj = exitSceneSignalSubj;
        }
    }
}
