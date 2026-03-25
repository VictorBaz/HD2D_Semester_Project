using UnityEngine;

[CreateAssetMenu(fileName = "AiAttackData", menuName = "Enemy/AiAttackData")]
public class EnemyAttackData : ScriptableObject
{
    [field: SerializeField] public float AttackCooldown { get; private set; } = 1f;
    [field: SerializeField] public float AnticipationTime { get; private set; } = 0.4f;
    [field: SerializeField] public float HitboxActiveDuration { get; private set; }
    [field: SerializeField] public float AttackDashSpeed { get; private set; }
    [field: SerializeField] public float AttackDashDuration { get; private set; }
}