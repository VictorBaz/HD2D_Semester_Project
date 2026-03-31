using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public NavigationSettings Navigation;
    public AttackSettings Attack;
    public StatusSettings Status; 
    
    public VisualSettings Visuals;

    public EnemyDataInstance Init() => new EnemyDataInstance(this);
}

#region Serialized Settings Structures

[System.Serializable]
public class NavigationSettings
{
    public float PatrolSpeed = 2f;
    public float ChaseSpeed = 4.5f;
    public float Acceleration = 8f;
    public float StoppingDistance = 1.2f;
    public float DetectionRange = 15f;
    public float ViewAngle = 90f;
    public float SearchDuration = 10f;
    public float SearchRadius = 5f;
}

[System.Serializable]
public class AttackSettings
{
    public float AttackCooldown = 1f;
    public float AnticipationTime = 0.4f;
    public float HitboxActiveDuration = 0.2f;
    public float AttackDashSpeed = 5f;
    public float AttackDashDuration = 0.2f;
}

[System.Serializable]
public class StatusSettings
{
    [Header("K-O & Stun")]
    public int MaxKo = 100;
    public float KoTime = 15f;
    public float StunDuration = 0.2f;
    
    [Header("Exposed State")]
    public float ExposedTime = 1f;
    
    [Header("Damage")]
    public int DamageToApply = 1;
}

[System.Serializable]
public class VisualSettings
{
    public Sprite SpriteSearch;
    public Sprite SpriteAttackStart;
    public Sprite SpriteChase;
    public Sprite SpritePatrol;
    public Sprite SpriteKo;
    public Sprite SpriteFall;
    public Sprite SpriteTakeDamage;
    public Sprite SpriteExposed;
}

#endregion

[System.Serializable]
public class EnemyDataInstance
{
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