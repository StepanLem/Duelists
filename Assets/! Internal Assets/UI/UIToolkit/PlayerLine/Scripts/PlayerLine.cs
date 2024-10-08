using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private const string IsReadyText = "Готов";
    private const string NotReadyText = "Не готов";

    [ContextMenu(nameof(SetPlayer))]
    public void SetPlayer()
    {
        SetPlayer("D9eka", true, false);
    }

    [ContextMenu(nameof(SetAnotherPlayer))]
    public void SetAnotherPlayer()
    {
        SetPlayer("whoiman", false, true);
    }

    [ContextMenu(nameof(RemovePlayer))]
    public void RemovePlayer()
    {
        SetState(PlayerLineState.WithoutPlayer);
    }

    public void SetPlayer(string nickname, bool isHost, bool isReady)
    {
        _nicknameText.text = nickname;
        _readyText.text = isReady ? IsReadyText : NotReadyText;
        _hostIcon.gameObject.SetActive(isHost);

        SetState(PlayerLineState.WithPlayer);
    }

    public void SetReadyState(bool isReady)
    {
        if (_state == PlayerLineState.WithoutPlayer)
        {
            return;
        }

        _readyText.text = isReady ? IsReadyText : NotReadyText;
    }

    private void Awake()
    {
        SetState(_initialState);
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
