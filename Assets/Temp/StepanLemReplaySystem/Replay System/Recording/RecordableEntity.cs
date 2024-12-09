using System;
using System.Collections.Generic;
using UnityEngine;

public class RecordableEntity : MonoBehaviour
{
    [SerializeField] private List<RecorderComponent> _recorders;

    [Tooltip("Нужен для привязки записи к объекту во время его сериализации/десериализации.")]
    public int TargetDataID;

    [Tooltip("if null, use RecordingSystem._defaultTicker")]
    [SerializeField] private MonoTicker _defaultTicker;
    private bool _usedDefaultTicker;

    public void StartRecording(GameRecord gameRecord, MonoTicker defaultTicker, Func<float> recordingTimeGetter)
    {
        var entityRecord = new EntityRecord();
        gameRecord.AddEntityRecordByID(entityRecord, TargetDataID);

        if (_defaultTicker == null) { _defaultTicker = defaultTicker; _usedDefaultTicker = true;  }

        foreach (var recorder in _recorders)
        {
            recorder.StartRecording(entityRecord, _defaultTicker, recordingTimeGetter);
        }
    }

    public void StopRecording()
    {
        foreach (var recorder in _recorders)
        {
            recorder.StopRecording();
        }

        if (_usedDefaultTicker) { _defaultTicker = null; _usedDefaultTicker = false; }
    }
}
