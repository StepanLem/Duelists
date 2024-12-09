using System;
using UnityEngine;

public abstract class MonoTicker : MonoBehaviour, ITicker
{
    private int _subscriberCount;
    protected event Action _onTick;

    public event Action Fire 
    {
        add
        {
            _onTick += value;
            _subscriberCount++;
            if (!enabled)
            {
                enabled = true;
            }
        }
        remove
        {
            _onTick -= value;
            _subscriberCount--;
            if (enabled && _subscriberCount == 0)
            {
                enabled = false;
            }
        }
    }

    protected void Tick()
    {
        _onTick?.Invoke();
    }
}