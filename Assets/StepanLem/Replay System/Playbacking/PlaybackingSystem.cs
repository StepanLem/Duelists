using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackingSystem : MonoBehaviour
{
    [SerializeField] private List<PlaybackableTarget> _playbackableTargets;

    [SerializeField] private MonoTicker _defaultTicker;

    public bool IsPlaybacking { get; private set; }
    public event Action OnPlaybackingCompleted;

    private int _completedTargetPlaybacksCount;

    public void StartPlaybacking(RecordedGameData recordedGameData)
    {
        if (IsPlaybacking)
        {
            Debug.LogWarning("Проигрывание уже идёт");
            return;
        }

        IsPlaybacking = true;

        _completedTargetPlaybacksCount = 0;

        foreach (var playbackableTarget in _playbackableTargets)
        {
            playbackableTarget.OnPlaybackCompleted += OnTargetPlaybackingCompleted;
            playbackableTarget.StartPlaybacking(recordedGameData, _defaultTicker);
        }
    }

    private void OnTargetPlaybackingCompleted()
    {
        _completedTargetPlaybacksCount++;
        if (_completedTargetPlaybacksCount == _playbackableTargets.Count)
            FinishPlaybacking();
    }

    private void FinishPlaybacking()
    {
        foreach (var playbackableTarget in _playbackableTargets)
        {
            playbackableTarget.OnPlaybackCompleted -= FinishPlaybacking;
        }

        OnPlaybackingCompleted?.Invoke();

        IsPlaybacking = false;
        Debug.LogWarning("Проигрывание завершено");
    }
}
