using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StartSoloGameScreen : MonoBehaviour
{
    [SerializeField] private Button[] _levelButtons;

    private UISceneRootBinder _uiSceneRootBinder;
    private MatchManager _matchManager;

    [Inject]
    public void Construct(UISceneRootBinder uiSceneRootBinder, MatchManager matchManager)
    {
        _uiSceneRootBinder = uiSceneRootBinder;
        _matchManager = matchManager;
    }

    public void StartMatch(int matchNum)
    {
        int matchsCound = _levelButtons.Length;
        _matchManager.StartMatch(matchNum, matchsCound);
        _uiSceneRootBinder.LoadScene(SceneRegistry.GameplayScene);
    }
}
