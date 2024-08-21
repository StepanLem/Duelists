using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Sliceable : MonoBehaviour
{
    [SerializeField]
    private SliceLogicBase _sliceLogic;

    public bool TrySlice(Vector3 sliceNormal, Vector3 sliceOrigin)
    {
        return _sliceLogic.TrySlice(sliceNormal, sliceOrigin);
    }
}
