using UnityEngine;

[CreateAssetMenu(fileName = "LockOnData", menuName = "Player/Data/LockOnData")]
public class LockOnData : ScriptableObject
{
    [field: Header("Detection")]
    [field: SerializeField] public float LockRange { get; private set; } = 15f;
    [field: SerializeField] public float LockAngle { get; private set; } = 90f;
    [field: SerializeField] public LayerMask LockableLayer { get; private set; }

    [field: Header("Rotation")]
    [field: Tooltip("Slerp speed when rotating toward the locked target")]
    [field: SerializeField] public float RotationSpeed { get; private set; } = 10f;
}