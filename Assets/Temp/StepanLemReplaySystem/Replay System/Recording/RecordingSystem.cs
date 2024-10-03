using System;
using System.Collections.Generic;
using UnityEngine;

public interface IRecordingDurationProvider
{
    public float GetCurrentRecordingDuration();
}

public class RecordingSystem : MonoBehaviour, IRecordingDurationProvider
{
    [SerializeField] private List<RecordableTarget> _recordableTargets;

    [SerializeField] private MonoTicker _defaultTicker;

    public bool IsRecording { get; private set; }
    public event Action<RecordedGameData> OnRecordingCompleted;

    private RecordedGameData _currentRecordedGameData;

    [Tooltip("Last Recording Start Time in seconds relative to Game Start Time")]
    private float _lastRecordingStartTime;

    public void StartRecording()
    {
        if (IsRecording)
        {
            Debug.LogWarning("Запись уже идёт");
            return;
        }

        IsRecording = true;

        _currentRecordedGameData = new();
        _lastRecordingStartTime = Time.realtimeSinceStartup;

        foreach (var recordableTarget in _recordableTargets)
        {
            recordableTarget.StartRecording(_currentRecordedGameData, _defaultTicker, this);
        }
    }

    public float GetCurrentRecordingDuration() => Time.realtimeSinceStartup - _lastRecordingStartTime;

    public void StopRecording()
    {
        if (!IsRecording)
        {
            Debug.LogWarning("Запись не велась");
            return;
        }

        foreach (var recordableTarget in _recordableTargets)
        {
            recordableTarget.StopRecording();
        }

        _currentRecordedGameData.Duration = GetCurrentRecordingDuration();

        OnRecordingCompleted?.Invoke(_currentRecordedGameData);

        _currentRecordedGameData=null;
        IsRecording = false;
    }
}
