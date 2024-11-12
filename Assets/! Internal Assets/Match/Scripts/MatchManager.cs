using System;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private Enemy[] _enemies;

    private int _currentMatch;
    private int _matchsCount;

    public Action OnStartNextMatch;
    public Action OnEndMatch;

    public int MatchNum => _currentMatch;
    public bool IsLastLevel => _currentMatch == _matchsCount - 1;
    public Enemy Enemy => _enemies[_currentMatch];

    public void StartMatch(int matchNum, Enemy[] enemies)
    {
        _enemies = enemies;
        _matchsCount = enemies.Length;
        StartMatch(matchNum);
    }

    public void StartMatch(int matchNum, int matchsCount)
    {
        _matchsCount = matchsCount;
        StartMatch(matchNum);
    }

    private void StartMatch(int matchNum)
    {
        _currentMatch = matchNum;
    }

    public void StartNextMatch()
    {
        _currentMatch++;
    }
}
