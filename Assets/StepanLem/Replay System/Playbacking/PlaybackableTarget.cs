using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackableTarget : MonoBehaviour
{
    [SerializeField] private List<PlaybackableValue> _playbackableValues;

    [Tooltip("Нужен для привязки записи к объекту во время его сериализации/десериализации.")]
    public int TargetDataID;

    [Tooltip("if null, use PlaybackingSystem._defaultTicker")]
    [SerializeField] private MonoTicker _defaultTicker;
    private bool _usedDefaultTicker;

    public event Action OnPlaybackCompleted;

    private int _completedValuePlaybacksCount;

    public void StartPlaybacking(RecordedGameData recordedGameData, MonoTicker defaultTicker)
    {
        var currentPlaybackingTargetData = recordedGameData.GetRecordedTargetDataByInstanceID(TargetDataID);

        if (currentPlaybackingTargetData == null)
        {
            //Debug.Log("В RecordedData нет записи о объекте");
            OnPlaybackCompleted?.Invoke();
            return;
        }

        if (_playbackableValues.Count == 0)
        {
            Debug.LogWarning("У PlaybackableTarget нет ни одного PlaybackableValue. Нет чему выставлять значения.");
            OnPlaybackCompleted?.Invoke();
            return;
        }

        _completedValuePlaybacksCount = 0;
        if (_defaultTicker == null) { _defaultTicker = defaultTicker; _usedDefaultTicker = true; }
        
        foreach (var playbackableValue in _playbackableValues)
        {
            playbackableValue.OnPlaybackCompleted += OnValuePlaybackingCompleted;
            playbackableValue.StartPlaybacking(currentPlaybackingTargetData, _defaultTicker);
        }
    }

    private void OnValuePlaybackingCompleted()
    {
        _completedValuePlaybacksCount++;
        if (_completedValuePlaybacksCount == _playbackableValues.Count)
            FinishPlaybacking();
    }

    public void FinishPlaybacking()
    {
        foreach (var playbackableValue in _playbackableValues)
        {
            playbackableValue.OnPlaybackCompleted -= OnValuePlaybackingCompleted;
        }

        if (_usedDefaultTicker) { _defaultTicker = null; _usedDefaultTicker = false; }

        OnPlaybackCompleted?.Invoke();
    }
}
