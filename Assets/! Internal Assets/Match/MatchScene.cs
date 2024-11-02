using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class MatchScene : MonoBehaviour
{
    [SerializeField] private UnityEvent OnNextRound;
    [SerializeField] private UnityEvent OnEndMatch;

    private MatchManager _matchManager;

    [Inject]
    public void Construct(MatchManager matchManager)
    {
        _matchManager = matchManager;
    }

    private void Start()
    {
        _matchManager.StartNextRound += () => OnNextRound?.Invoke();
        _matchManager.EndMatch += () => OnEndMatch?.Invoke();
    }
}
