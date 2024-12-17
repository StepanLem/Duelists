using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Zenject;

public class PlayerLine : MonoBehaviour
{
    public enum PlayerLineState
    {
        WithoutPlayer,
        WithPlayer
    }

    [SerializeField] private PlayerLineState _initialState;
    [SerializeField] private bool _hostView;
    [Space]
    [SerializeField] private TextMeshProUGUI _inviteText;
    [SerializeField] private Button _inviteButton;
    [Space]
    [SerializeField] private GameObject _playerInfoContainer;
    [SerializeField] private TextMeshProUGUI _nicknameText;
    [SerializeField] private TextMeshProUGUI _readyText;
    [Space]
    [SerializeField] private Image _hostIcon;
    [SerializeField] private Button _giveHostButton;
    [SerializeField] private Button _kickPlayerButton;

    private PlayerLineState _state;

    private string _playerNickname;
    private bool _isHost;
    private bool _isReady;

    private LocalizationManager _localizationManager;

    private const string ReadyLocale = "PlayerLine_Ready";
    private const string NotReadyLocale = "PlayerLine_Not_Ready";

    [ContextMenu(nameof(SetPlayer))]
    public void SetPlayer()
    {
        SetPlayer("Player1", true, false);
    }

    [ContextMenu(nameof(SetAnotherPlayer))]
    public void SetAnotherPlayer()
    {
        SetPlayer("Player2", false, true);
    }

    [ContextMenu(nameof(RemovePlayer))]
    public void RemovePlayer()
    {
        SetState(PlayerLineState.WithoutPlayer);
    }

    [Inject]
    public void Cunstruct(LocalizationManager localizationManager)
    {
        _localizationManager = localizationManager;
    }

    public void SetPlayer(string nickname, bool isHost, bool isReady)
    {
        _playerNickname = nickname;
        _isHost = isHost;
        _isReady = isReady;

        SetPlayerVisual();
    }

    private void SetPlayerVisual()
    {
        _nicknameText.text = _playerNickname;
        _readyText.text = _localizationManager.GetField(_isReady ? ReadyLocale : NotReadyLocale);
        _hostIcon.gameObject.SetActive(_isHost);

        SetState(PlayerLineState.WithPlayer);
    }

    public void SetReadyState(bool isReady)
    {
        if (_state == PlayerLineState.WithoutPlayer)
        {
            return;
        }

        _readyText.text = isReady ? ReadyLocale : NotReadyLocale;
    }

    private void Awake()
    {
        SetState(_initialState);
        LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
    }

    private void LocalizationSettings_SelectedLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        SetPlayerVisual();
    }

    private void SetState(PlayerLineState state)
    {
        _inviteText.gameObject.SetActive(false);
        _inviteButton.interactable = false;
        _playerInfoContainer.SetActive(false);

        if (state == PlayerLineState.WithoutPlayer)
        {
            _inviteText.gameObject.SetActive(true);
            _inviteButton.interactable = true;
        }
        else
        {
            _playerInfoContainer.SetActive(true);
            if (_hostView)
            {
                _giveHostButton.gameObject.SetActive(true);
                _kickPlayerButton.gameObject.SetActive(true);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_playerInfoContainer.GetComponent<RectTransform>());
        }
        _state = state;
    }
}
