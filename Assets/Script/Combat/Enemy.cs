using Assets.Script.Combat.EventArgs;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool IsStartAttacking = false;
    public void Kill()
    {
        EventBus.Trigger("EnemyKilled", new KillArgs() { AttackStarted = IsStartAttacking });
    }
}
