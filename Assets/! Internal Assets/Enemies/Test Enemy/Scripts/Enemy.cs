using System;
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour
{
    private Match _match;

    private bool _canBeAttacked;

    [Inject]
    public void Construct(Match match)
    {
        _match = match;
    }

    private void Awake()
    {
        _match.OnAttackStart += Match_OnAttackStart;
        _match.OnAttackEnd += Match_OnAttackEnd;
    }

    private void Match_OnAttackStart()
    {
        SetCanBeAttackedState(true);
    }

    private void Match_OnAttackEnd()
    {
        SetCanBeAttackedState(false);
    }

    private void SetCanBeAttackedState(bool state)
    {
        _canBeAttacked = state;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Check if it was player collision

        _match.EndMatch(_canBeAttacked);
    }

    private void OnDestroy()
    {
        _match.OnAttackStart -= Match_OnAttackStart;
        _match.OnAttackEnd -= Match_OnAttackEnd;
    }
}
