using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceLogicForNotAnimatedMeshRedrerer : SliceLogicBase
{
    [SerializeField]
    private Slice _sliceComponent;

    public override bool TrySlice(Vector3 sliceNormal, Vector3 sliceOrigin)
    {
        var fragments = _sliceComponent.ComputeSlice(sliceNormal, sliceOrigin);

        return fragments != null;
    }

    private void Reset()
    {
        if (!gameObject.TryGetComponent<Slice>(out _sliceComponent))
            _sliceComponent = gameObject.AddComponent<Slice>();


    }
}
