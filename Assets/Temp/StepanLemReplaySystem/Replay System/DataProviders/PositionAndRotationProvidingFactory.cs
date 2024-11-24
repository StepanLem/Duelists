using System;
using UnityEngine;

/// Этот класс можно было бы разбить на два монобеха для лучшей читаемости, но тогда каждый раз для каждого значения надо было бы два компонента создавать.
public class PositionAndRotationProvidingFactory : MonoBehaviour, IDataProvider<PositionAndRotation>, IRecorderFactory, IReaderFactory
{
    #region DataProvider
    [SerializeField] private Transform _transform;

    public PositionAndRotation GetValue() => new(_transform.position, _transform.rotation);

    public void SetValue(PositionAndRotation newValue)//сделать работать ссылочно?
    {
        _transform.SetPositionAndRotation(newValue.Position, newValue.Rotation);//ссылочно ли работает?
    }
    #endregion

    #region Factory, that connects Product with DataProvider
    public IDataRecorder CreateRecorder(Func<float> recordingTimeGetter)
    {
        return new DataRecorderWithGetter<PositionAndRotation>(GetValue, recordingTimeGetter);
    }

    public IRecordReader CreateReader()
    {
        return new RecordReaderWithSetter<PositionAndRotation>(SetValue);
    }
    #endregion
}

public readonly struct PositionAndRotation
{
    public PositionAndRotation(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }

    public readonly Vector3 Position;
    public readonly Quaternion Rotation;
}