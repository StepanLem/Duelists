using System;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private Enemy[] _enemies;
    [SerializeField] private RoundSO _roundData;

    private int _roundsCount;
    private int _currentRound;

    public Action StartNextRound;
    public Action EndMatch;

    public int RoundNum => _currentRound;
    public Enemy Enemy => _enemies[_currentRound];
    public RoundSO RoundData => _roundData;

    private void Awake()
    {
        //_roundCount = _enemies.Length;
        _roundsCount = 0;
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

    public void EndRound()
    {
        _currentRound++;
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
}
