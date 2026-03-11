using UnityEngine;

[CreateAssetMenu(fileName = "CombatData", menuName = "Player/Data/CombatData")]
public class CombatData : ScriptableObject
{
    [field: Header("Dash")]
    [field: SerializeField] public float DashSpeed { get; private set; } = 6f;
    [field: SerializeField] public float DashDuration { get; private set; } = 0.35f;

    [field: Header("Attack")]
    [field: SerializeField] public AnimationClip AttackClip { get; private set; }

    [field: Header("Combo")]
    [field: Tooltip("Time window after an attack during which a combo input is accepted")]
    [field: SerializeField] public float ComboWindow { get; private set; } = 0.5f;

    public float GetAttackClipLength() => AttackClip != null ? AttackClip.length : 0f;
}