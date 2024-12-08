using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Editor;
using UnityEngine;
using Zenject;

public class PlayerRecorder : MonoBehaviour
{
    private IRecordingTarget<PlayerData> _recordingTarget;
    private IPlaybackTarget<PlayerData> _playbackTarget;
    private IPlayerDataProvider _playerDataProvider;
    private IRecordingController<PlayerData> _activeRecording;
    private ITicker _ticker;
    private IReplayReader<PlayerData> _replayReader;

    [Inject]
    public void Construct(
        IRecordingTargetFactory recordingTargetFactory, 
        IPlaybackTargetFactory playbackTargetFactory,
        IPlayerDataProvider playerDataProvider,
        UpdateTicker updateTicker)
    {
        _recordingTarget = recordingTargetFactory.CreateTarget(() => playerDataProvider.GetData());
        _playbackTarget = playbackTargetFactory.CreateTarget<PlayerData>(playerData => playerDataProvider.SetData(playerData));
        _ticker = updateTicker;
    }

    [ContextMenu("start recording")]
    public void StartRecording()
    {
        _activeRecording = _recordingTarget.StartRecording(_ticker);
    }

    [ContextMenu("save recording")]
    public void StopRecording()
    {
        if (_activeRecording is null)
        {
            return;
        }
        _activeRecording.Stop();
        _replayReader = _activeRecording.GetReplay();
    }

    [ContextMenu("start playback")]
    public void StartPlayback()
    {
        if (_replayReader is null)
        {
            return;
        }
        var playback = _playbackTarget.StartPlayback(_replayReader, _ticker);
        playback.OnFinish += () => Debug.Log("replay ended");
    }
}