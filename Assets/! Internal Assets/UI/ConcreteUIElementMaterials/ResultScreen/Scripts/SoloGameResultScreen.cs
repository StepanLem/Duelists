using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class SoloGameResultScreen : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [Space]
    [SerializeField] private TextMeshProUGUI _resultText;
    [Space]
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _reloadButton;
    [SerializeField] private Button _continueButton;

    private UISceneRootBinder _uiSceneRootBinder;
    private Match _match;
    private MatchManager _matchManager;

    private const string WinResultText = "Вы выиграли!";
    private const string LoseResultText = "Вы проиграли!";

    private const string ContinueButtonDefaultText = "Продолжить";
    private const string ContinueButtonReloadText = "Перезагрузить";
    private const string ContinueButtonLastLevelText = "Завершить";

    [Inject]
    public void Construct(UISceneRootBinder uiSceneRootBinder, Match match, MatchManager matchManager)
    {
        _uiSceneRootBinder = uiSceneRootBinder;
        _match = match;
        _matchManager = matchManager;
    }

    private void Start()
    {
        _match.OnMatchEnd += (bool isPlayerWin) => Setup(isPlayerWin);
    }

    private void Setup(bool isPlayerWin)
    {
        bool isLastLevel = _matchManager.IsLastLevel;
        _resultText.text = isPlayerWin ? WinResultText : LoseResultText;

        SetupButton(_exitButton, !(isPlayerWin && isLastLevel), 
            () => _uiSceneRootBinder.LoadScene(SceneRegistry.MainMenuScene));
        SetupButton(_reloadButton, isPlayerWin, 
            () => _uiSceneRootBinder.LoadScene(SceneRegistry.GameplayScene));
        SetupContinueButton(isPlayerWin, isLastLevel);

        _content.SetActive(true);
    }

    private void SetupButton(Button button, bool isActive, UnityAction call)
    {
        button.gameObject.SetActive(isActive);
        if (isActive)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(call);
        }
    }

    private void SetupContinueButton(bool isPlayerWin, bool isLastLevel)
    {
        _continueButton.onClick.RemoveAllListeners();
        TextMeshProUGUI continueButtonText = _continueButton.GetComponentInChildren<TextMeshProUGUI>();
        if (isPlayerWin)
        {
            if (isLastLevel)
            {
                continueButtonText.text = ContinueButtonLastLevelText;
                _continueButton.onClick.AddListener(() => _uiSceneRootBinder.LoadScene(SceneRegistry.MainMenuScene));
            }
            else
            {
                continueButtonText.text = ContinueButtonDefaultText;
                _continueButton.onClick.AddListener(() => _matchManager.StartNextMatch());
                _continueButton.onClick.AddListener(() => _uiSceneRootBinder.LoadScene(SceneRegistry.GameplayScene));
            }
        }
        else
        {
            continueButtonText.text = ContinueButtonReloadText;
            _continueButton.onClick.AddListener(() => _uiSceneRootBinder.LoadScene(SceneRegistry.GameplayScene));
        }
    }

    private void OnDestroy()
    {
        _match.OnMatchEnd -= (bool isPlayerWin) => Setup(isPlayerWin);
    }
}
