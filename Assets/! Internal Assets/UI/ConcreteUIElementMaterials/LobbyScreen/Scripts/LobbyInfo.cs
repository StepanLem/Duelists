using TMPro;
using UnityEngine;

public class LobbyInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyCodeText;
    [SerializeField] private TextMeshProUGUI _gameModeText;
    [SerializeField] private TextMeshProUGUI _lobbyModeText;
    [SerializeField] private TextMeshProUGUI _roundsCountText;
    [Space]
    [SerializeField] private SegmentedButton _gameModeButton;
    [SerializeField] private SegmentedButton _lobbyModeButton;
    [SerializeField] private SegmentedButton _roundsCountButton;

    public void SetInfo(string lobbyCode, string gameMode, string lobbyMode, string roundsCount)
    {
        _lobbyCodeText.text = lobbyCode;
        _gameModeText.text = gameMode;
        _lobbyModeText.text = lobbyMode;
        _roundsCountText.text = roundsCount;
    }

    private void OnEnable()
    {
        _gameModeText.text = _gameModeButton.Value;
        _lobbyModeText.text = _lobbyModeButton.Value;
        _roundsCountText.text = _roundsCountButton.Value;
    }
}
