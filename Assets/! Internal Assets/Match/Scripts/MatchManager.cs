using System;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private Enemy[] _enemies;
    [SerializeField] private RoundSO _roundData;

    private int _roundsCount;
    private int _currentRound;

    private int _playerScore;
    private int _enemyScore;

    public Action StartNextRound;
    public Action EndMatch;
    public Action<(int, int)> UpdateScore;

    public int RoundNum => _currentRound;
    public Enemy Enemy => _enemies[_currentRound];
    public RoundSO RoundData => _roundData;

    public (int, int) GetScore()
    {
        return (_playerScore, _enemyScore);
    }

    public void StartMatch(Enemy[] enemies, RoundSO roundData)
    {
        _enemies = enemies;
        _roundsCount = enemies.Length; 
        _roundData = roundData;
    }

    public void StarMatch(int roundsCount, RoundSO roundData)
    {
        Debug.Log("Start 0 Round");
        _roundsCount = roundsCount;
        _roundData = roundData;
    }

    public void EndRound(bool isPlayerWin)
    {
        _currentRound++;
        OnUpdateScore(isPlayerWin);
    }

    public void OnStartNextRound()
    {
        if (_currentRound < _roundsCount)
        {
            Debug.Log($"Start {_currentRound} Round");
            StartNextRound?.Invoke();
        }
        else
        {
            Debug.Log($"End Match");
            EndMatch?.Invoke();
        }
    }

    private void OnUpdateScore(bool isPlayerWin)
    {
        if (isPlayerWin)
        {
            _playerScore++;
        }
        else
        {
            _enemyScore++;
        }
        UpdateScore?.Invoke((_playerScore, _enemyScore));
    }
}
