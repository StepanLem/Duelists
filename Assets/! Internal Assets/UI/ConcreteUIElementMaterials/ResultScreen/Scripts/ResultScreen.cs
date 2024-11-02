using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _winnerNickname;
    [SerializeField] private TextMeshProUGUI _playerScore;
    [SerializeField] private TextMeshProUGUI _enemyScore;
    [Space]
    [SerializeField] private Button _reloadMatchButton;

    private MatchManager _matchManager;

    [Inject]
    public void Construct(MatchManager matchManager)
    {
        _matchManager = matchManager;
    }

    private void Start()
    {
        _reloadMatchButton.onClick.AddListener(() => _matchManager.ReloadMatch());
    }

    private void OnEnable()
    {
        (int, int) score = _matchManager.GetScore();
        if (score.Item1 > score.Item2)
        {
            _winnerNickname.text = "Игрок";
        }
        else if (score.Item1 < score.Item2)
        {
            _winnerNickname.text = "Враг";
        }
        else
        {
            _winnerNickname.text = "Ничья";
        }
        _playerScore.text = score.Item1.ToString();
        _enemyScore.text = score.Item2.ToString();
    }
}
