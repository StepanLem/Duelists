using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class MatchScene : MonoBehaviour
{
    [SerializeField] private UnityEvent OnNextRound;
    [SerializeField] private UnityEvent OnEndMatch;
    [Space]
    [SerializeField] private Button _attackButton;

    private MatchManager _matchManager;
    private Round _round;

    [Inject]
    public void Construct(MatchManager matchManager, Round round)
    {
        _matchManager = matchManager; 
        _round = round;
    }

    private void Start()
    {
        _matchManager.StartNextRound += () => OnNextRound?.Invoke();
        _matchManager.EndMatch += () => OnEndMatch?.Invoke();

        _attackButton.gameObject.SetActive(false);
        _round.StartAttackRoundTime += () => _attackButton.gameObject.SetActive(true);
        _round.EndAttackRoundTime += () => _attackButton.gameObject.SetActive(false);
    }
}
