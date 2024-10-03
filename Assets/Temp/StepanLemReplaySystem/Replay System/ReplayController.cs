using System.Collections.Generic;
using UnityEngine;

public class ReplayController : MonoBehaviour
{
    public RecordingSystem RecordingSystem;
    public PlaybackingSystem PlaybackingSystem;

    public List<RecordedGameData> Replays = new();

    [Tooltip("Index of choosen replay in Replays")]
    public int ChoosenReplayIndex;

    public void OnEnable()
    {
        RecordingSystem.OnRecordingCompleted += SaveReplay;
    }

    private void SaveReplay(RecordedGameData replay)
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
        RecordingSystem.StartRecording();
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
