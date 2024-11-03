using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour
{
    private Animator _animator;

    private Round _round;

    private bool _canBeAttacked;

    private const string FakeAttackBool = "FakeAttack";
    private const string AttackTrigger = "Attack";

    [Inject]
    public void Construct(Round round)
    {
        _round = round;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _round.OnStartBeforeAttackRoundTime += () => _animator.SetBool(FakeAttackBool, true);
        _round.OnEndBeforeAttackRoundTime += () => _animator.SetBool(FakeAttackBool, false);
        _round.OnStartAfterAttackRoundTime += () => _animator.SetTrigger(AttackTrigger);
        _round.OnEndRound += () => _animator.SetBool(FakeAttackBool, false);

        _round.OnStartAttackRoundTime += () => _canBeAttacked = true;
        _round.OnEndAttackRoundTime += () => _canBeAttacked = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Check if it was player collision

        _round.EndRound(_canBeAttacked);
    }
}
