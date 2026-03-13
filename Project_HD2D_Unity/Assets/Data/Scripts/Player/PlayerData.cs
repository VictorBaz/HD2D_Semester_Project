using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    [field: SerializeField] public MovementData Movement { get; private set; }
    [field: SerializeField] public CombatData Combat   { get; private set; }
    [field: SerializeField] public ShootingData Shooting { get; private set; }
    [field: SerializeField] public LockOnData LockOn   { get; private set; }
    [field: SerializeField] public DashData DashData   { get; private set; }
    [field: SerializeField] public JumpData JumpData   { get; private set; }
    [field: SerializeField] public PlayerResourcesData PlayerResourcesData   { get; private set; }
    [field: SerializeField] public CarryData CarryData   { get; private set; }

    public PlayerDataInstance Init() => new PlayerDataInstance(this);
}

[System.Serializable]
public class PlayerDataInstance
{
    public float MoveSpeedWalking;
    public float MoveSpeedRunning;
    public float MoveSpeedSlope;
    public float RotationSpeed;
    public LayerMask GroundMask;
    public float GroundCheckDistance;
    public float PlayerHeight;
    public float MaxSlopeAngle;

    public CombatHitData[] ComboHits;
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
    
    public float DashSpeed;
    public float DashDuration;
    public float DashCooldown;

    public float JumpForce;
    public float JumpCooldown;
    public float MaxVerticalVelocity;
    public float GravityMultiplier;
    public float MaxGravityTime;
    public float JumpCutMultiplier;

    public int Life;
    public int MaxLife;
    public int Energy;
    public int MaxEnergy;

    public float CarryRange;
    public float CarryAngle;
    public LayerMask CarryLayer;

    public PlayerDataInstance(PlayerData data)
    {
        MoveSpeedWalking = data.Movement.MoveSpeedWalking;
        MoveSpeedRunning = data.Movement.MoveSpeedRunning;
        MoveSpeedSlope = data.Movement.MoveSpeedSlope;
        RotationSpeed = data.Movement.RotationSpeed;
        GroundMask = data.Movement.GroundMask;
        GroundCheckDistance = data.Movement.GroundCheckDistance;
        PlayerHeight = data.Movement.PlayerHeight;
        MaxSlopeAngle = data.Movement.MaxSlopeAngle;
        Acceleration = data.Movement.Acceleration;
        Deceleration = data.Movement.Deceleration;
        RunThreshold = data.Movement.RunThreshold;

        ComboHits = data.Combat.ComboHits;
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

        DashDuration = data.DashData.DashDuration;
        DashSpeed = data.DashData.DashSpeed;
        DashCooldown = data.DashData.DashCooldown;

        JumpForce = data.JumpData.JumpForce;
        JumpCooldown = data.JumpData.JumpCooldown;
        MaxVerticalVelocity = data.JumpData.MaxVerticalVelocity;
        GravityMultiplier = data.JumpData.GravityMultiplier;
        MaxGravityTime = data.JumpData.MaxGravityTime;
        JumpCutMultiplier = data.JumpData.JumpCutMultiplier;
        
        Energy = data.PlayerResourcesData.Energy;
        MaxEnergy = data.PlayerResourcesData.MaxEnergy;
        Life = data.PlayerResourcesData.Life;
        MaxLife = data.PlayerResourcesData.MaxLife;
        
        CarryRange = data.CarryData.CarryRange;
        CarryAngle = data.CarryData.CarryAngle;
        CarryLayer = data.CarryData.CarryLayer;
    }

    public float GetAttackClipLength(int index) => ComboHits[index].Clip != null ? ComboHits[index].Clip.length : 0f;

    public bool IsPlayerDead() => Life <= 0;
    
    public bool IsEnergyEmpty() => Energy <= 0;

    public void RemoveEnergy() => Energy--;
    public void AddEnergy() => Energy++;
    
    
}