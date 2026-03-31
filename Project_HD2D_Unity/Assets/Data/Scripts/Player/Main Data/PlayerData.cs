using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    public MovementSettings Movement;
    public CombatSettings Combat;
    public AbilitySettings Abilities; 
    public ResourceSettings Resources;

    public PlayerDataInstance Init() => new PlayerDataInstance(this);
}

#region Serialized Settings Structures

[System.Serializable]
public class MovementSettings
{
    [Header("Speeds")]
    public float MoveSpeedWalking = 5f;
    public float MoveSpeedRunning = 8f;
    public float MoveSpeedSlope = 8f;
    
    [Space(3)]
    
    public float SpeedMultiplierCarry = 0.5f;
    
    [Space(3)]
    
    public float RotationSpeed = 10f;
    public float Acceleration = 20f;
    public float Deceleration = 30f;
    public float RunThreshold = 0.6f;

    [Header("Ground & Slopes")]
    public LayerMask GroundMask;
    
    [Space(3)]
    
    public float GroundCheckDistance = 0.2f;
    public float PlayerHeight = 2f;
    public float MaxSlopeAngle = 45f;
}

[System.Serializable]
public class CombatSettings
{
    [Header("Melee Combo")]
    public float ComboWindow = 0.5f;
    public CombatHitData[] ComboHits;

    [Header("Parry")]
    public AnimationClip ParryAnimationClip;
    public float ParryHitboxStartOffset = 0.1f;
    public float ParryActiveDuration = 0.2f;
    public float ParryCooldown = 0.5f;

    [Header("Lock On")]
    public float LockRange = 15f;
    public float LockAngle = 90f;
    public LayerMask LockableLayer;
    public float LockRotationSpeed = 10f;

    [Header("Take Damage")] 
    public float HitDuration = 0.5f;
    public float HitForceTaken = 5f;
}

[System.Serializable]
public class AbilitySettings
{
    [Header("Jump")]
    public float JumpHeight = 2.0f;
    public float JumpDuration = 0.4f;
    public float JumpCooldown = 0.2f;
    public float GravityMultiplier = 3f;
    public float MaxGravityTime = 0.8f;
    public float CoyoteTime = 0.2f;
    public float AirControlForce = 0.2f;
    public float FallAndJumpThreshold = 0.1f;

    [Header("Dash")]
    public float DashSpeed = 20f;
    public float DashDuration = 0.4f;
    public float DashCooldown = 0.6f;

    [Header("Carry")]
    public float CarryRange = 3f;
    public float CarryAngle = 90f;
    public LayerMask CarryLayer;
}

[System.Serializable]
public class ResourceSettings
{
    public int MaxLife = 3;
    public int MaxEnergy = 5;
    public int StartingLife = 3;
    public int StartingEnergy = 0;
}

[System.Serializable]
public class CombatHitData
{
    [Header("Animation")]
    public AnimationClip Clip;
    
    [Header("Mouvement")]
    public float DashSpeed;
    public float DashStartOffset;
    public float DashDuration;

    [Header("Timings Hitbox")]
    [Tooltip("Temps avant l'activation du collider")]
    public float HitboxStartOffset; 
    [Tooltip("Durée pendant laquelle le collider reste actif")]
    public float HitboxActiveDuration;
}

#endregion

[System.Serializable]
public class PlayerDataInstance
{
    public float MoveSpeedWalking;
    public float MoveSpeedRunning;
    public float MoveSpeedSlope;
    public float CarrySpeedMultiplier;
    public float RotationSpeed;
    public float Acceleration;
    public float Deceleration;
    public float RunThreshold;
    public LayerMask GroundMask;
    public float GroundCheckDistance;
    public float PlayerHeight;
    public float MaxSlopeAngle;
    public float CoyoteTime;
    
    public CombatHitData[] ComboHits;
    public AnimationClip ParryAnimationClip;
    public float ParryHitboxStartOffset;
    public float ParryActiveDuration;
    public float ParryCooldown;
    public float LockRange;
    public float LockAngle;
    public LayerMask LockableLayer;
    public float LockRotationSpeed;
    public float HitDuration;
    public float HitForceTaken;

    public float JumpHeight;
    public float JumpDuration;
    public float JumpCooldown;
    public float AirControlForce;
    public float FallAndJumpThreshold;
    public float GravityMultiplier;
    public float MaxGravityTime;
    public float DashSpeed;
    public float DashDuration;
    public float DashCooldown;
    public float CarryRange;
    public float CarryAngle;
    public LayerMask CarryLayer;

    public int Life;
    public int MaxLife;
    public int Energy;
    public int MaxEnergy;

    public PlayerDataInstance(PlayerData data)
    {
        MoveSpeedWalking = data.Movement.MoveSpeedWalking;
        MoveSpeedRunning = data.Movement.MoveSpeedRunning;
        MoveSpeedSlope = data.Movement.MoveSpeedSlope;
        CarrySpeedMultiplier = data.Movement.SpeedMultiplierCarry;
        RotationSpeed = data.Movement.RotationSpeed;
        Acceleration = data.Movement.Acceleration;
        Deceleration = data.Movement.Deceleration;
        RunThreshold = data.Movement.RunThreshold;
        GroundMask = data.Movement.GroundMask;
        GroundCheckDistance = data.Movement.GroundCheckDistance;
        PlayerHeight = data.Movement.PlayerHeight;
        MaxSlopeAngle = data.Movement.MaxSlopeAngle;

        ComboHits = data.Combat.ComboHits;
        ParryAnimationClip = data.Combat.ParryAnimationClip;
        ParryHitboxStartOffset = data.Combat.ParryHitboxStartOffset;
        ParryActiveDuration = data.Combat.ParryActiveDuration;
        ParryCooldown = data.Combat.ParryCooldown;
        LockRange = data.Combat.LockRange;
        LockAngle = data.Combat.LockAngle;
        LockableLayer = data.Combat.LockableLayer;
        LockRotationSpeed = data.Combat.LockRotationSpeed;
        HitDuration = data.Combat.HitDuration;
        HitForceTaken = data.Combat.HitForceTaken;
        
        JumpHeight = data.Abilities.JumpHeight;
        JumpDuration = data.Abilities.JumpDuration;
        JumpCooldown = data.Abilities.JumpCooldown;
        AirControlForce =  data.Abilities.AirControlForce;
        FallAndJumpThreshold =  data.Abilities.FallAndJumpThreshold;
        GravityMultiplier = data.Abilities.GravityMultiplier;
        MaxGravityTime = data.Abilities.MaxGravityTime;
        CoyoteTime = data.Abilities.CoyoteTime;
        DashSpeed = data.Abilities.DashSpeed;
        DashDuration = data.Abilities.DashDuration;
        DashCooldown = data.Abilities.DashCooldown;
        CarryRange = data.Abilities.CarryRange;
        CarryAngle = data.Abilities.CarryAngle;
        CarryLayer = data.Abilities.CarryLayer;

        MaxLife = data.Resources.MaxLife;
        MaxEnergy = data.Resources.MaxEnergy;
        Life = data.Resources.StartingLife;
        Energy = data.Resources.StartingEnergy;
    }

    public float GetAttackClipLength(int index) => (ComboHits != null && index < ComboHits.Length && ComboHits[index].Clip != null) ? ComboHits[index].Clip.length : 0f;
    public bool IsPlayerDead() => Life <= 0;
    public bool IsEnergyEmpty() => Energy <= 0;
    public void RemoveEnergy() => Energy = Mathf.Max(0, Energy - 1);
    public void AddEnergy() => Energy = Mathf.Min(MaxEnergy, Energy + 1);
}