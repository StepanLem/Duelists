using System;
using System.Collections.Generic;
using UnityEngine;

public class RecordingSystem : MonoBehaviour
{
    [SerializeField] private List<RecordableEntity> _recordableEntities;

    [SerializeField] private MonoTicker _defaultTicker;

    public bool IsRecording { get; private set; }
    public event Action<GameRecord> OnRecordingCompleted;

    private GameRecord _currentGameRecord;

    [Tooltip("Last Recording Start Time in seconds relative to Game Start Time")]
    private float _lastRecordingStartTime;

    public void StartRecording(GameRecord gameRecord)
    {
        if (IsRecording)
        {
            Debug.LogWarning("Запись уже идёт");
            return;
        }

        IsRecording = true;

        _currentGameRecord = gameRecord;
        _lastRecordingStartTime = Time.realtimeSinceStartup;

        foreach (var recordableTarget in _recordableEntities)
        {
            recordableTarget.StartRecording(_currentGameRecord, _defaultTicker, GetCurrendRecordingTime);
        }
    }

    public float GetCurrendRecordingTime() => Time.realtimeSinceStartup - _lastRecordingStartTime;

    public void StopRecording()
    {
        if (!IsRecording)
        {
            Debug.LogWarning("Запись не велась");
            return;
        }

        foreach (var recordableTarget in _recordableEntities)
        {
            recordableTarget.StopRecording();
        }

        //_currentReplayRecording.SetRecordParameters()
        //_currentReplayRecording.SetRecordHeader(GetCurrendRecordingTime()):
        _currentGameRecord.Duration = GetCurrendRecordingTime();

        OnRecordingCompleted?.Invoke(_currentGameRecord);

        _currentGameRecord=null;
        IsRecording = false;
    }
}
