﻿using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class Round : MonoBehaviour
{
    private MatchManager _matchManager;

    public Action StartRound;
    public Action StartBeforeAttackRoundTime;
    public Action EndBeforeAttackRoundTime;
    public Action StartAttackRoundTime;
    public Action EndAttackRoundTime;
    public Action StartAfterAttackRoundTime;
    public Action EndAfterAttackRoundTime;
    public Action EndRound;

    [Inject]
    public void Construct(MatchManager matchManager)
    {
        _matchManager = matchManager;
    }

    public void StopRound(bool isPlayerWin)
    {
        StopAllCoroutines();
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

    private void Start()
    {
        //SPAWN ENEMY
        StartCoroutine(RoundRountine(_matchManager.RoundData));
    }

    private IEnumerator RoundRountine(RoundSO round)
    {
        StartRound?.Invoke();
        yield return StartCoroutine(RoundTimeRoutine(round.BeforeAttackTime, StartBeforeAttackRoundTime, EndBeforeAttackRoundTime));
        yield return StartCoroutine(RoundTimeRoutine(round.AttackTime, StartAttackRoundTime, EndAttackRoundTime));
        Debug.Log("You LOSE!");
        _matchManager.EndRound(false);
        EndRound?.Invoke();
        yield return StartCoroutine(RoundTimeRoutine(round.AfterAttackTime, StartAfterAttackRoundTime, EndAfterAttackRoundTime));
        _matchManager.OnStartNextRound();
    }

    private IEnumerator RoundTimeRoutine(float roundTime, Action beforeRoundAction, Action afterRoundAction)
    {
        beforeRoundAction?.Invoke();
        //Debug.Log($"{nameof(beforeRoundAction)}");
        yield return new WaitForSeconds(roundTime);
        afterRoundAction?.Invoke();
        //Debug.Log($"{nameof(afterRoundAction)}");
    }

    private IEnumerator StopRoundRoutine(bool isPlayerWin)
    {
        _matchManager.EndRound(isPlayerWin);
        EndRound?.Invoke();
        yield return new WaitForSeconds(_matchManager.RoundData.AfterAttackTime);
        _matchManager.OnStartNextRound();
    }
}
