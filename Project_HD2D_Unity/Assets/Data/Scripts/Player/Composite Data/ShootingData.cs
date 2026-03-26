using UnityEngine;

[CreateAssetMenu(fileName = "ShootingData", menuName = "Player/Data/ShootingData")]
public class ShootingData : ScriptableObject
{
    [field: Header("Charge")]
    [field: Tooltip("Hold duration under this threshold = quick shot")]
    [field: SerializeField] public float ChargeThreshold { get; private set; } = 0.2f;

    [field: Tooltip("Duration to reach max charge")]
    [field: SerializeField] public float MaxChargeTime { get; private set; } = 2f;

    [field: Header("Projectile Selection")]
    [field: Tooltip("Charge ratio below this = medium projectile, above = heavy projectile")]
    [field: SerializeField] public float MediumHeavyThreshold { get; private set; } = 0.5f;

    [field: Header("Movement Penalty")]
    [field: Tooltip("Speed multiplier applied at max charge (0 = full stop, 1 = no penalty)")]
    [field: SerializeField] public float MinSpeedMultiplier { get; private set; } = 0.2f;

    [field: Header("Deadzone")]
    [field: Tooltip("Minimum stick magnitude to update shoot direction")]
    [field: SerializeField] public float InputDeadzone { get; private set; } = 0.8f;
}