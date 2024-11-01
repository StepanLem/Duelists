using UnityEngine;

[CreateAssetMenu(fileName = "RoundSO", menuName = "Duelists/Round")]
public class RoundSO : ScriptableObject
{
    [SerializeField] private float _beforeAttackTime;
    [SerializeField] private float _attackTime;
    [SerializeField] private float _afterAttackTime;

    public float BeforeAttackTime => _beforeAttackTime;
    public float AttackTime => _attackTime;
    public float AfterAttackTime => _afterAttackTime;
}
