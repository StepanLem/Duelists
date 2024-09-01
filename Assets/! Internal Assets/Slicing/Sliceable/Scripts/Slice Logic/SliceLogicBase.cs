using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SliceLogicBase : MonoBehaviour
{
    public abstract bool TrySlice(Vector3 sliceNormal, Vector3 sliceOrigin);
}
