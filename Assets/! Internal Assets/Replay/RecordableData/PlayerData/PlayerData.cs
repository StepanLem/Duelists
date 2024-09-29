using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public readonly struct PlayerData
{
    public readonly struct Transform
    {
        public Transform(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public readonly Vector3 Position;
        public readonly Quaternion Rotation;

        public static Transform Lerp(Transform a, Transform b, double t)
        {
            var ft = (float)t;
            return new Transform(
                Vector3.Lerp(a.Position, b.Position, ft),
                Quaternion.Lerp(a.Rotation.normalized, b.Rotation.normalized, ft));
        }
    }

    public readonly Transform LeftHand;
    public readonly Transform RightHand;
    public readonly Transform Head;
    public PlayerData(Transform leftHand, Transform rightHand, Transform head)
    {
        LeftHand = leftHand;
        RightHand = rightHand;
        Head = head;
    }

    public static PlayerData Lerp(PlayerData a, PlayerData b, double t)
    {
        return new PlayerData(
            Transform.Lerp(a.LeftHand, b.LeftHand, t),
            Transform.Lerp(a.RightHand, b.RightHand, t),
            Transform.Lerp(a.Head, b.Head, t)
            );
    }
}