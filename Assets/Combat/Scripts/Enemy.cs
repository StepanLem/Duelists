using Assets.Script.Combat.EventArgs;
using R3;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool IsStartedAttacking = false;
    public void Die()
    {
        EventBus.Trigger("EnemyDied", new EnemyDeathArgs() { IsStartedAttacking = IsStartedAttacking });
    }
}
