using System;
using UnityEngine;

public class OnTriggerComponent : MonoBehaviour
{
    public event Action<Collider> OnEnter;
    public event Action<Collider> OnExit;

    public void OnTriggerEnter(Collider other)
    {
        OnEnter?.Invoke(other);
    }
    
    public void OnTriggerExit(Collider other)
    {
        OnExit?.Invoke(other);
    }
}
