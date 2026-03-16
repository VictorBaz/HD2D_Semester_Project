using UnityEngine;

[CreateAssetMenu(fileName = "AiAttackData", menuName = "Enemy/AiAttackData")]
public class AiAttackData : ScriptableObject
{
    [field: SerializeField] public float AttackCooldown { get; private set; } = 1f;
    [field: SerializeField] public float KnockbackStrength { get; private set; } = 15f;
    [field: SerializeField] public float AnticipationTime { get; private set; } = 0.4f;
}