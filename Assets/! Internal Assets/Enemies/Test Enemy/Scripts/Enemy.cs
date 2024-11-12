using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour
{
    private Animator _animator;

    private Match _match;

    private bool _canBeAttacked;

    private const string FakeAttackBool = "FakeAttack";
    private const string AttackTrigger = "Attack";

    [Inject]
    public void Construct(Match match)
    {
        _match = match;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _match.OnMatchStart += () => _animator.SetBool(FakeAttackBool, true);
        _match.OnAttackStart += () => _animator.SetBool(FakeAttackBool, false);
        _match.OnAttackEnd += () => _animator.SetTrigger(AttackTrigger);
        _match.OnMatchEnd += (bool isPlayerWin) => _animator.SetBool(FakeAttackBool, false);

        _match.OnAttackStart += () => _canBeAttacked = true;
        _match.OnAttackEnd += () => _canBeAttacked = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Check if it was player collision

        _match.EndMatch(_canBeAttacked);
    }

    private void OnDestroy()
    {
        _match.OnMatchStart -= () => _animator.SetBool(FakeAttackBool, true);
        _match.OnAttackStart -= () => _animator.SetBool(FakeAttackBool, false);
        _match.OnAttackEnd -= () => _animator.SetTrigger(AttackTrigger);
        _match.OnMatchEnd -= (bool isPlayerWin) => _animator.SetBool(FakeAttackBool, false);

        _match.OnAttackStart -= () => _canBeAttacked = true;
        _match.OnAttackEnd -= () => _canBeAttacked = false;
    }
}
