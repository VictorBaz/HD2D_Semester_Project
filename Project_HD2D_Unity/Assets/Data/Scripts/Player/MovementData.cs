using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "Player/Data/MovementData")]
public class MovementData : ScriptableObject
{
    [field: Header("Speed")]
    [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;
    [field: SerializeField] public float RotationSpeed { get; private set; } = 10f;

    [field: Header("Jump")]
    [field: SerializeField] public float JumpForce { get; private set; } = 5f;

    [field: Header("Ground Detection")]
    [field: SerializeField] public LayerMask GroundMask { get; private set; }
    [field: SerializeField] public float GroundCheckDistance { get; private set; } = 0.2f;
    [field: SerializeField] public float PlayerHeight { get; private set; } = 2f;

    [field: Header("Slope")]
    [field: SerializeField] public float MaxSlopeAngle { get; private set; } = 45f;
}