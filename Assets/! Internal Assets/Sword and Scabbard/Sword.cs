using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Sword : MonoBehaviour
{
    public Collider[] Colliders;

    public Transform Tip;

    public SphereCollider EndSwordTrigger;//for test

    public Rigidbody Rigidbody;

    public XRGrabInteractable XRGrabInteractable;
}