using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionAndRotationValueProvider : IValueProvider
{
    [SerializeField] private Transform _transform;

    public override IValue GetValue()
    {
        return new PositionAndRotation(_transform.position, _transform.rotation);
    }

    public override void SetValue(in IValue value)
    {
        var posAndRot = (PositionAndRotation)value;
        _transform.SetPositionAndRotation(posAndRot.Position, posAndRot.Rotation);
    }

    public struct PositionAndRotation : IValue
    {
        public PositionAndRotation(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
    }

}
