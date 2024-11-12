using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InGameScreen : MonoBehaviour
{
    [SerializeField] private Button _attackButton;

    private MatchManager _matchManager;
    private Match _match;

    [Inject]
    public void Construct(MatchManager matchManager, Match match)
    {
        _matchManager = matchManager;
        _match = match;
    }

    private void Awake()
    {
        _attackButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        _attackButton.onClick.AddListener(() => _match.EndMatch(true));
        _match.OnAttackStart += () => _attackButton.gameObject.SetActive(true);
        _match.OnAttackEnd += () => _attackButton.gameObject.SetActive(false);
        _match.OnMatchEnd += (bool isPlayerWin) => _attackButton.gameObject.SetActive(false);
    }
}
