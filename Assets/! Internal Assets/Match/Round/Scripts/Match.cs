using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class Match : MonoBehaviour
{
    private MatchManager _matchManager;

    private bool _isMatchFinished;

    public event Action OnMatchStart;
    public event Action OnAttackStart;
    public event Action OnAttackEnd;
    public event Action<bool> OnMatchEnd;

    [Inject]
    public void Construct(MatchManager matchManager)
    {
        _matchManager = matchManager;
    }

    public void StartMatch()
    {
        OnMatchStart?.Invoke();
    }

    public void StartAttack()
    {
        OnAttackStart?.Invoke();
    }

    public void EndAttack()
    {
        OnAttackEnd?.Invoke();
    }

    public void EndMatch(bool isPlayerWin)
    {
        if (_isMatchFinished)
        {
            return;
        }
        _isMatchFinished = true;

        if (isPlayerWin)
        {
            Debug.Log("You WIN!");
        }
        else
        {
            Debug.Log("You LOSE!");
        }
        StartCoroutine(EndMatchRoutine(isPlayerWin));
    }


    private IEnumerator EndMatchRoutine(bool isPlayerWin)
    {
        Debug.Log(_matchManager.MatchNum);
        yield return new WaitForSeconds(3f);
        OnMatchEnd?.Invoke(isPlayerWin);
    }
}
