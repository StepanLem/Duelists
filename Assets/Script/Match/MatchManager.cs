using Assets.Script.Combat.EventArgs;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MatchManager : MonoBehaviour
{
    public UnityEvent OnVictory;
    public UnityEvent OnDefeat;
    void Start()
    {
        EventBus.Register<KillArgs>("EnemyKilled", HandleKill);
    }

    public void HandleKill(KillArgs args)
    {
        if (args.AttackStarted)
        {
            OnVictory?.Invoke();
        }
        else
        {
            OnDefeat?.Invoke();
        }
    }
}
