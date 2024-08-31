using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class MatchManager : MonoBehaviour
{
    public UnityEvent OnVictory;
    public UnityEvent OnDefeat;

    private Enemy _enemy;

    [Inject]
    public void Construct(Enemy enemy)
    {
        _enemy = enemy;
    }

    private void OnEnable()
    {
        _enemy.OnDeath += HandleEnemyDeath;
    }

    private void OnDisable()
    {
        _enemy.OnDeath -= HandleEnemyDeath;
    }

    public void HandleEnemyDeath()
    {
        if (_enemy.IsStartedAttacking)
        {
            OnVictory?.Invoke();
        }
        else
        {
            OnDefeat?.Invoke();
        }
    }
}
