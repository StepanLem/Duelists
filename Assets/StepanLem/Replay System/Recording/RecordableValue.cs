using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordableValue : MonoBehaviour
{
    [SerializeField] private IValueProvider _valueProvider;

    [Tooltip("if null, use RecordableTarget._defaultTicker")]
    [SerializeField] private MonoTicker _ticker;

    /// <summary>
    /// ID нужен для привязки записи к объекту во время его сериализации/десериализации.
    /// </summary>
    public int InstanceID;

    private RecordedValueData _currentRecordingValueData;
    private RecordingSystem _recordingSystem;

    public void StartRecording(RecordedTargetData _currentRecordingTargetData, MonoTicker defaultTicker, RecordingSystem recordingSystem)
    {
        _currentRecordingValueData = new RecordedValueData();
        _currentRecordingTargetData.AddRecordedValueDataByInstanceID(_currentRecordingValueData, InstanceID);

        if (_ticker == null) _ticker = defaultTicker;
        _recordingSystem = recordingSystem;

        _ticker.Fire += MakeSnapshot;
    }

    public void StopRecording()
    {
        _ticker.Fire -= MakeSnapshot;
    }

    public void MakeSnapshot()
    {
        SnapshotOfValue snapshot = new(_valueProvider.GetValue(), _recordingSystem.GetTimeFromRecordingStart());
        _currentRecordingValueData.Snapshots.Add(snapshot);
    }
}
