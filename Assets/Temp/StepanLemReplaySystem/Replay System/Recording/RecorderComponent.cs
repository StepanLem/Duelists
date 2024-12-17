using System;
using UnityEngine;

public class RecorderComponent : MonoBehaviour
{
    [Tooltip("Для получения значения фабрика должена создавать именно DataRecorderWithGetter<T>")]
    [SerializeField, InterfaceType(typeof(IRecorderFactory))]
    private UnityEngine.Object _factoryOfRecorderWithGetter;
    private IRecorderFactory RecorderFactory => _factoryOfRecorderWithGetter as IRecorderFactory;

    [Tooltip("Нужен для привязки записи к объекту во время его сериализации/десериализации.")]
    public int ValueDataID;

    [Tooltip("If null, use defaultTicker")]
    [SerializeField] private MonoTicker _ticker;
    private bool _usedDefaultTicker;

    public IDataRecorder Recorder;

    public void StartRecording(EntityRecord entityRecord, MonoTicker defaultTicker, Func<float> recordingTimeGetter)
    {
        Recorder = RecorderFactory.CreateRecorder(recordingTimeGetter);

        var dataRecord = Recorder.CreateDataRecord();
        entityRecord.AddDataRecordByID(dataRecord, ValueDataID);

        if (_ticker == null) { _ticker = defaultTicker; _usedDefaultTicker = true; }

        _ticker.Fire += Recorder.MakeSnapshot;
    }

    public void StopRecording()
    {
        _ticker.Fire -= Recorder.MakeSnapshot;

        if (_usedDefaultTicker) { _ticker = null; _usedDefaultTicker = false; }
        Recorder = null;
    }
}

public interface IDataRecorder
{
    public IDataRecord CreateDataRecord();
    public void MakeSnapshot();
}

public class DataRecorderWithGetter<T> : IDataRecorder where T : struct
{
    private Func<T> _valueGetter;
    private Func<float> _recordingTimeGetter;

    private DataRecord<T> _dataRecord;

    public DataRecorderWithGetter(Func<T> valueGetter, Func<float> recordingTimeGetter)
    {
        _valueGetter = valueGetter;
        _recordingTimeGetter = recordingTimeGetter;
    }

    public IDataRecord CreateDataRecord()
    {
        return _dataRecord = new();
    }

    public void MakeSnapshot()
    {
        _dataRecord.Snapshots.Add(new Snapshots<T>(_valueGetter(), _recordingTimeGetter()));
    }    
}