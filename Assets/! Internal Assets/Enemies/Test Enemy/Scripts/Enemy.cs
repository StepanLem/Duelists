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

        _round.StartBeforeAttackRoundTime += () => _animator.SetBool(FakeAttackBool, true);
        _round.EndBeforeAttackRoundTime += () => _animator.SetBool(FakeAttackBool, false);
        _round.StartAfterAttackRoundTime += () => _animator.SetTrigger(AttackTrigger);
        _round.EndRound += () => _animator.SetBool(FakeAttackBool, false);

        _round.StartAttackRoundTime += () => _canBeAttacked = true;
        _round.EndAttackRoundTime += () => _canBeAttacked = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Check if it was player collision

        _round.StopRound(_canBeAttacked);
    }
}
