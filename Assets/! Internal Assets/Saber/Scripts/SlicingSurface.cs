using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class SlicingSurface : MonoBehaviour
{
    [SerializeField] private Transform _slicingPlane;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<Sliceable>(out var sliceable))
            return;

        sliceable.TrySlice(_slicingPlane.transform.right, _slicingPlane.transform.position);
    }
}
