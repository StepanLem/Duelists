using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;
using Zenject;

public class Round : MonoBehaviour
{
    private MatchManager _matchManager;

    private bool _isRoundFinished;

    public event Action OnStartRound;
    public event Action OnStartBeforeAttackRoundTime;
    public event Action OnEndBeforeAttackRoundTime;
    public event Action OnStartAttackRoundTime;
    public event Action OnEndAttackRoundTime;
    public event Action OnStartAfterAttackRoundTime;
    public event Action OnEndAfterAttackRoundTime;
    public event Action OnEndRound;

    [Inject]
    public void Construct(MatchManager matchManager)
    {
        _matchManager = matchManager;
    }

    public void StartRound()
    {
        OnStartRound?.Invoke();
    }

    public void StartBeforeAttackRoundTime()
    {
        OnStartBeforeAttackRoundTime?.Invoke();
    }

    public void EndBeforeAttackRoundTime()
    {
        OnEndBeforeAttackRoundTime?.Invoke();
    }

    public void StartAttackRoundTime()
    {
        OnStartAttackRoundTime?.Invoke();
    }

    public void EndAttackRoundTime()
    {
        OnEndAttackRoundTime?.Invoke();
    }

    public void StartAfterAttackRoundTime()
    {
        OnStartAfterAttackRoundTime?.Invoke();
    }

    public void EndAfterAttackRoundTime()
    {
        OnEndAfterAttackRoundTime?.Invoke();
    }

    public void EndRound(bool isPlayerWin)
    {
        if (_isRoundFinished)
        {
            return;
        }
        _isRoundFinished = true;

        if (isPlayerWin)
        {
            Debug.Log("You WIN!");
        }
        else
        {
            Debug.Log("You LOSE!");
        }
        StartCoroutine(StopRoundRoutine(isPlayerWin));
    }


    private IEnumerator StopRoundRoutine(bool isPlayerWin)
    {
        _matchManager.EndRound(isPlayerWin);
        OnEndRound?.Invoke();
        yield return new WaitForSeconds(3f);
        _matchManager.OnStartNextRound();
    }
}
