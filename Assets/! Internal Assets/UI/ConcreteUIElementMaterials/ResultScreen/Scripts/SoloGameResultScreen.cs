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
    private LocalizationManager _localizationManager;

    private const string WinResultText = "SoloGameResultScreen/WinResultText";
    private const string LoseResultText = "SoloGameResultScreen/LoseResultText";

    private const string ContinueButtonDefaultText = "SoloGameResultScreen/ContinueButtonDefaultText";
    private const string ContinueButtonReloadText = "SoloGameResultScreen/ContinueButtonReloadText";
    private const string ContinueButtonLastLevelText = "SoloGameResultScreen/ContinueButtonLastLevelText";

    [Inject]
    public void Construct(UISceneRootBinder uiSceneRootBinder, Match match, 
                          MatchManager matchManager, LocalizationManager localizationManager)
    {
        _uiSceneRootBinder = uiSceneRootBinder;
        _match = match;
        _matchManager = matchManager;
        _localizationManager = localizationManager;
    }

    private void Start()
    {
        _match.OnMatchEnd += Match_OnMatchEnd;
    }

    private void Match_OnMatchEnd(bool isPlayerWin)
    {
        Setup(isPlayerWin);
    }

    private void Setup(bool isPlayerWin)
    {
        bool isLastLevel = _matchManager.IsLastLevel;
        _resultText.text = _localizationManager.GetField(isPlayerWin ? WinResultText : LoseResultText);

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
                continueButtonText.text = _localizationManager.GetField(ContinueButtonLastLevelText);
                _continueButton.onClick.AddListener(() => _uiSceneRootBinder.LoadScene(SceneRegistry.MainMenuScene));
            }
            else
            {
                continueButtonText.text = _localizationManager.GetField(ContinueButtonDefaultText);
                _continueButton.onClick.AddListener(() => _matchManager.StartNextMatch());
                _continueButton.onClick.AddListener(() => _uiSceneRootBinder.LoadScene(SceneRegistry.GameplayScene));
            }
        }
        else
        {
            continueButtonText.text = _localizationManager.GetField(ContinueButtonReloadText);
            _continueButton.onClick.AddListener(() => _uiSceneRootBinder.LoadScene(SceneRegistry.GameplayScene));
        }
    }

    private void OnDestroy()
    {
        _match.OnMatchEnd -= Match_OnMatchEnd;
    }
}
