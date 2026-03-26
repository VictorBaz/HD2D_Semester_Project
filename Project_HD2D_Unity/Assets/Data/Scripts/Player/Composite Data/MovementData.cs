using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "Player/Data/MovementData")]
public class MovementData : ScriptableObject
{
    [field: Header("Speed")]
    [field: SerializeField] public float MoveSpeedWalking { get; private set; } = 5f;
    [field: SerializeField] public float MoveSpeedRunning { get; private set; } = 5f;
    [field: SerializeField] public float MoveSpeedSlope { get; private set; } = 5f;
    [field: SerializeField] public float RotationSpeed { get; private set; } = 10f;

    [field: Header("Ground Detection")]
    [field: SerializeField] public LayerMask GroundMask { get; private set; }
    [field: SerializeField] public float GroundCheckDistance { get; private set; } = 0.2f;
    [field: SerializeField] public float PlayerHeight { get; private set; } = 2f;

    [field: Header("Slope")]
    [field: SerializeField] public float MaxSlopeAngle { get; private set; } = 45f;
    
    
    [field: Header("Acceleration")]
    [field: SerializeField] public float Acceleration   { get; private set; } = 20f;
    [field: SerializeField] public float Deceleration   { get; private set; } = 30f;

    [field: Header("Speed Threshold")]
    [field: Tooltip("Input magnitude above which run speed is applied")]
    [field: SerializeField] public float RunThreshold   { get; private set; } = 0.6f;
    
    [field: SerializeField] public float CoyotteTime { get; private set; } = 0.2f;
}