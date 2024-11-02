using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InGameScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerScore;
    [SerializeField] private TextMeshProUGUI _enemyScore;

    [SerializeField] private Button _attackButton;

    private MatchManager _matchManager;
    private Round _round;

    [Inject]
    public void Construct(MatchManager matchManager, Round round)
    {
        _matchManager = matchManager;
        _round = round;
    }

    private void Awake()
    {
        _attackButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        UpdateScore(_matchManager.GetScore());
        _matchManager.UpdateScore += ((int, int) score) => UpdateScore(score);

        _attackButton.onClick.AddListener(() => _round.StopRound(true));
        _round.StartAttackRoundTime += () => _attackButton.gameObject.SetActive(true);
        _round.EndAttackRoundTime += () => _attackButton.gameObject.SetActive(false);
        _round.EndRound += () => _attackButton.gameObject.SetActive(false);
    }

    private void UpdateScore((int, int) score)
    {
        _playerScore.text = score.Item1.ToString();
        _enemyScore.text = score.Item2.ToString();
    }
}
