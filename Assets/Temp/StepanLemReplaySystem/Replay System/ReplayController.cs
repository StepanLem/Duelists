using System.Collections.Generic;
using UnityEngine;

public class ReplayController : MonoBehaviour
{
    public RecordingSystem RecordingSystem;
    public PlaybackingSystem PlaybackingSystem;

    public List<GameRecord> Replays = new();

    [Tooltip("Index of choosen replay in Replays")]
    public int ChoosenReplayIndex;

    private GameRecord _currentReplayRecording;

    public void OnEnable()
    {
        RecordingSystem.OnRecordingCompleted += SaveReplay;
    }

    private void SaveReplay(GameRecord replay)
    {
        Replays.Add(replay);
    }

    private void OnDisable()
    {
        RecordingSystem.OnRecordingCompleted -= SaveReplay;
    }

    [ContextMenu("Start Recording")]
    public void StartRecording()
    {
        _currentReplayRecording = new GameRecord();
        Replays.Add(_currentReplayRecording);

        RecordingSystem.StartRecording(_currentReplayRecording);
    }

    [ContextMenu("Stop Recording")]
    public void StopRecording()
    {
        RecordingSystem.StopRecording();
    }

    [ContextMenu("Start Playbacking")]
    public void StartPlaybacking()
    {
        PlaybackingSystem.StartPlaybacking(Replays[ChoosenReplayIndex]);
    }
}
