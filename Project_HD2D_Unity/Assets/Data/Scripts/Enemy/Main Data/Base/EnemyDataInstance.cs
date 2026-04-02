using UnityEngine;

[System.Serializable]
public class EnemyDataInstance
{
    public EnemyData BaseData;
    
    public float PatrolSpeed;
    public float ChaseSpeed;
    public float Acceleration;
    public float StoppingDistance;
    public float DetectionRange;
    public float ViewAngle;
    public float SearchDuration;
    public float SearchRadius;

    public float AttackCooldown;
    public float AnticipationTime;
    public float HitboxActiveDuration;
    public float AttackDashSpeed;
    public float AttackDashDuration;
    public float AttackRange;

    public float AttackJumpForce;
    public float GroundDetectionDistance;
    public float NavMeshSampleMargin;
    public float LandingStunDuration;
    public float ShockwaveActiveDuration;

    public int MaxKo;
    public float KoTime;
    public int CurrentKo;
    public float StunDuration;
    public float ExposedTime;
    public int DamageToApply;

    public Sprite SpriteSearch;
    public Sprite SpriteAttackStart;
    public Sprite SpriteChase;
    public Sprite SpritePatrol;
    public Sprite SpriteKo;
    public Sprite SpriteFall;
    public Sprite SpriteTakeDamage;
    public Sprite SpriteExposed;

    public EnemyDataInstance(EnemyData data)
    {
        PatrolSpeed = data.Navigation.PatrolSpeed;
        ChaseSpeed = data.Navigation.ChaseSpeed;
        Acceleration = data.Navigation.Acceleration;
        StoppingDistance = data.Navigation.StoppingDistance;
        DetectionRange = data.Navigation.DetectionRange;
        ViewAngle = data.Navigation.ViewAngle;
        SearchDuration = data.Navigation.SearchDuration;
        SearchRadius = data.Navigation.SearchRadius;

        AttackCooldown = data.Attack.AttackCooldown;
        AnticipationTime = data.Attack.AnticipationTime;
        HitboxActiveDuration = data.Attack.HitboxActiveDuration;
        AttackDashSpeed = data.Attack.AttackDashSpeed;
        AttackDashDuration = data.Attack.AttackDashDuration;

        AttackJumpForce = data.Attack.AttackJumpForce;
        GroundDetectionDistance = data.Attack.GroundDetectionDistance;
        NavMeshSampleMargin = data.Attack.NavMeshSampleMargin;
        LandingStunDuration = data.Attack.LandingStunDuration;
        ShockwaveActiveDuration = data.Attack.ShockwaveActiveDuration;
        AttackRange = data.Attack.AttackRange;

        MaxKo = data.Status.MaxKo;
        KoTime = data.Status.KoTime;
        StunDuration = data.Status.StunDuration;
        ExposedTime = data.Status.ExposedTime;
        DamageToApply = data.Status.DamageToApply;
        CurrentKo = 0;

        SpriteSearch = data.Visuals.SpriteSearch;
        SpriteAttackStart = data.Visuals.SpriteAttackStart;
        SpriteChase = data.Visuals.SpriteChase;
        SpritePatrol = data.Visuals.SpritePatrol;
        SpriteKo = data.Visuals.SpriteKo;
        SpriteFall = data.Visuals.SpriteFall;
        SpriteTakeDamage = data.Visuals.SpriteTakeDamage;
        SpriteExposed = data.Visuals.SpriteExposed;
    }

    public bool IsKoFull() => CurrentKo >= MaxKo;
    public void ResetKo() => CurrentKo = 0;

    public Sprite GetSpriteByStateName(string stateName)
    {
        return stateName switch
        {
            "Chase" => SpriteChase,
            "Searching" => SpriteSearch,
            "Attacking" => SpriteAttackStart,
            "Patrol" => SpritePatrol,
            "K-O" => SpriteKo,
            "Falling" => SpriteFall,
            "Taking Damage" => SpriteTakeDamage,
            "Exposed" => SpriteExposed,
            _ => SpritePatrol
        };
    }
}