using System;
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

        _match.OnAttackStart += Match_OnAttackStart;
        _match.OnAttackEnd += Match_OnAttackEnd;
        _match.OnMatchEnd += Match_OnMatchEnd;
    }

    private void Match_OnAttackStart()
    {
        SetAttackButtonState(true);
    }

    private void Match_OnAttackEnd()
    {
        SetAttackButtonState(false);
    }

    private void Match_OnMatchEnd(bool isPlayerWin)
    {
        SetAttackButtonState(false);
    }

    private void SetAttackButtonState(bool state)
    {
        _attackButton.gameObject.SetActive(state);
    }

    private void OnDestroy()
    {
        _match.OnAttackStart -= Match_OnAttackStart;
        _match.OnAttackEnd -= Match_OnAttackEnd;
        _match.OnMatchEnd -= Match_OnMatchEnd;
    }
}
