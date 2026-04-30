using UnityEngine;

[System.Serializable]
public class EnemyDataInstance
{
    public float PatrolSpeed;
    public float ChaseSpeed;
    public float Acceleration;
    public float StoppingDistance;
    public float SearchDuration;
    public float SearchRadius;
    public float RotationSpeed;
    public float GroundCheckDistance;
    public float NavmeshCheckDistance;

    public float AttackCooldown;
    public float HitboxActiveDuration;
    public float AttackDashSpeed;
    public float AttackDashDuration;

    public float AttackJumpForce;
    public float GroundDetectionDistance;
    public float NavMeshSampleMargin;
    public float LandingStunDuration;
    public float ShockwaveActiveDuration;

    public float KoTime;
    public float KoTimeMax;
    
    public int MaxKo;
    public int CurrentKo;
    
    public float StunDuration;
    
    public float ExposedTime;
    
    public AnimationClip ChargeAnimationClip;
    public AnimationClip AttackAnimationClip;

    public EnemyDataInstance(EnemyData data)
    {
        PatrolSpeed = data.Navigation.PatrolSpeed;
        ChaseSpeed = data.Navigation.ChaseSpeed;
        Acceleration = data.Navigation.Acceleration;
        StoppingDistance = data.Navigation.StoppingDistance;
        SearchDuration = data.Navigation.SearchDuration;
        SearchRadius = data.Navigation.SearchRadius;
        RotationSpeed = data.Navigation.RotationSpeed;
        GroundCheckDistance = data.Navigation.GroundCheckDistance;
        NavmeshCheckDistance = data.Navigation.NavmeshCheckDistance;

        AttackCooldown = data.Attack.AttackCooldown;
        HitboxActiveDuration = data.Attack.HitboxActiveDuration;
        AttackDashSpeed = data.Attack.AttackDashSpeed;
        AttackDashDuration = data.Attack.AttackDashDuration;

        AttackJumpForce = data.Attack.AttackJumpForce;
        GroundDetectionDistance = data.Attack.GroundDetectionDistance;
        NavMeshSampleMargin = data.Attack.NavMeshSampleMargin;
        LandingStunDuration = data.Attack.LandingStunDuration;
        ShockwaveActiveDuration = data.Attack.ShockwaveActiveDuration;

        MaxKo = data.Status.MaxKo;
        KoTime = 0;
        KoTimeMax =  data.Status.KoTimeMax;
        StunDuration = data.Status.StunDuration;
        ExposedTime = data.Status.ExposedTime;
        CurrentKo = 0;

        
        ChargeAnimationClip = data.Attack.ChargeAnimationClip;
        AttackAnimationClip = data.Attack.AttackAnimationClip;
    }

    public bool IsKoFull() => CurrentKo >= MaxKo;
    public bool IsKoTimerEmpty() => KoTime <= 0;
    public void ResetKo() => CurrentKo = 0;
    public void ResetKoTimer() => KoTime = 0;
    
    
    public float GetAnimationCLipLengthChargeAttack() => ChargeAnimationClip.length;
    public float GetAnimationCLipLengthAttack() => AttackAnimationClip.length;
}