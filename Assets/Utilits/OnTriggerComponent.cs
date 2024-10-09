using System;
using UnityEngine;

public class OnTriggerComponent : MonoBehaviour
{
    public Action<Collider> OnEnter;
    public Action<Collider> OnExit;

    public void OnTriggerEnter(Collider other)
    {
        OnEnter(other);
    }
    
    public void OnTriggerExit(Collider other)
    {
        OnExit(other);
    }
}
