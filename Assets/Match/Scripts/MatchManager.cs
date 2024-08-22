using Assets.Script.Combat.EventArgs;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using R3;

public class MatchManager : MonoBehaviour
{
    public UnityEvent OnVictory;
    public UnityEvent OnDefeat;

    private void Start()
    {
        EventBus.Register<EnemyDeathArgs>("EnemyDied", HandleDeath);
    }

    public void HandleDeath(EnemyDeathArgs args)
    {
        if (args.IsStartedAttacking)
        {
            OnVictory?.Invoke();
        }
        else
        {
            OnDefeat?.Invoke();
        }
    }
}
