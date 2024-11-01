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
        yield return StartCoroutine(RoundTimeRoutine(round.AfterAttackTime, StartAfterAttackRoundTime, EndAfterAttackRoundTime));
        EndRound?.Invoke();
        _matchManager.EndRound();
    }

    private IEnumerator RoundTimeRoutine(float roundTime, Action beforeRoundAction, Action afterRoundAction)
    {
        beforeRoundAction?.Invoke();
        //Debug.Log($"{nameof(beforeRoundAction)}");
        yield return new WaitForSeconds(roundTime);
        afterRoundAction?.Invoke();
        //Debug.Log($"{nameof(afterRoundAction)}");
    }

    private void StopRound()
    {
        StartCoroutine(StopRoundRoutine());
    }

    private IEnumerator StopRoundRoutine()
    {
        yield return new WaitForSeconds(_matchManager.RoundData.AfterAttackTime);
        EndRound?.Invoke();
        _matchManager.EndRound();
    }
}
