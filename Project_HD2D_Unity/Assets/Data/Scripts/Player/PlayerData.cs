using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    [field: SerializeField] public MovementData Movement { get; private set; }
    [field: SerializeField] public CombatData Combat   { get; private set; }
    [field: SerializeField] public ShootingData Shooting { get; private set; }
    [field: SerializeField] public LockOnData LockOn   { get; private set; }

    public PlayerDataInstance Init() => new PlayerDataInstance(this);
}

[System.Serializable]
public class PlayerDataInstance
{
    public float MoveSpeedWalking;
    public float MoveSpeedRunning;
    public float MoveSpeedSlope;
    public float RotationSpeed;
    public float JumpForce;
    public LayerMask GroundMask;
    public float GroundCheckDistance;
    public float PlayerHeight;
    public float MaxSlopeAngle;

    public float DashSpeed;
    public float DashDuration;
    public AnimationClip AttackClip;
    public float ComboWindow;

    public float ChargeThreshold;
    public float MaxChargeTime;
    public float MediumHeavyThreshold;
    public float MinSpeedMultiplier;
    public float InputDeadzone;

    public float LockRange;
    public float LockAngle;
    public LayerMask LockableLayer;
    public float LockOnRotationSpeed;
    public float Acceleration;
    public float Deceleration;
    public float RunThreshold;

    public PlayerDataInstance(PlayerData data)
    {
        MoveSpeedWalking = data.Movement.MoveSpeedWalking;
        MoveSpeedRunning = data.Movement.MoveSpeedRunning;
        MoveSpeedSlope = data.Movement.MoveSpeedSlope;
        RotationSpeed = data.Movement.RotationSpeed;
        JumpForce = data.Movement.JumpForce;
        GroundMask = data.Movement.GroundMask;
        GroundCheckDistance = data.Movement.GroundCheckDistance;
        PlayerHeight = data.Movement.PlayerHeight;
        MaxSlopeAngle = data.Movement.MaxSlopeAngle;
        Acceleration = data.Movement.Acceleration;
        Deceleration = data.Movement.Deceleration;
        RunThreshold = data.Movement.RunThreshold;

        DashSpeed = data.Combat.DashSpeed;
        DashDuration = data.Combat.DashDuration;
        AttackClip = data.Combat.AttackClip;
        ComboWindow = data.Combat.ComboWindow;

        ChargeThreshold = data.Shooting.ChargeThreshold;
        MaxChargeTime = data.Shooting.MaxChargeTime;
        MediumHeavyThreshold = data.Shooting.MediumHeavyThreshold;
        MinSpeedMultiplier = data.Shooting.MinSpeedMultiplier;
        InputDeadzone = data.Shooting.InputDeadzone;

        LockRange = data.LockOn.LockRange;
        LockAngle = data.LockOn.LockAngle;
        LockableLayer = data.LockOn.LockableLayer;
        LockOnRotationSpeed = data.LockOn.RotationSpeed;
    }

    public float GetAttackClipLength() => AttackClip != null ? AttackClip.length : 0f;
}