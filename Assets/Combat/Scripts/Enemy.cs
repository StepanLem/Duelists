using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool IsStartedAttacking = false;
    public Action OnDeath;

    [ContextMenu("Die")]
    public void Die()
    {
        //logic

        OnDeath?.Invoke();
    }
}
