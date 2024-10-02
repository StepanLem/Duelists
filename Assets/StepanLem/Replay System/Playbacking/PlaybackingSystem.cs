using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackingSystem : MonoBehaviour
{
    [SerializeField] private List<PlaybackableTarget> _playbackableTargets;

    public MonoTicker DefaultTicker;

    public event Action OnPlaybackingCompleted;

    public bool IsPlaybacking { get; private set; }

    public void StartPlaybacking(RecordedGameData recordedGameData)
    {
        if (IsPlaybacking)
        {
            Debug.LogWarning("Проигрывание уже идёт");
            return;
        }

        foreach (var playbackableTarget in _playbackableTargets)
        {
            playbackableTarget.StartPlaybacking(recordedGameData, DefaultTicker);
            playbackableTarget.OnPlaybackCompleted += FinishPlaybacking;
        }

        IsPlaybacking = true;
    }

    private void FinishPlaybacking()
    {
        foreach (var playbackableTarget in _playbackableTargets)
        {
            playbackableTarget.OnPlaybackCompleted -= FinishPlaybacking;
        }

        IsPlaybacking = false;

        OnPlaybackingCompleted?.Invoke();

        Debug.LogWarning("Проигрывание завершено");
    }
}
