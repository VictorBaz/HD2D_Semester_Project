using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [field: SerializeField] public EnemyNavigationData EnemyNavigationData { get; private set; } // AJOUT
    [field: SerializeField] public EnemyAttackData EnemyAttackData { get; private set; }
    [field: SerializeField] public EnemyKOData EnemyKoData { get; private set; }
    [field: SerializeField] public EnemySearchData EnemySearchData { get; private set; }
    [field: SerializeField] public EnemyTakeDamageData EnemyTakeDamageData { get; private set; }
    [field: SerializeField] public EnemyDataFeedBack EnemyDataFeedBack { get; private set; }
    [field: SerializeField] public EnemyExposedData EnemyExposedData { get; private set; }
    
    public EnemyDataInstance Init() => new EnemyDataInstance(this);
}

public class EnemyDataInstance
{
    public float PatrolSpeed;
    public float ChaseSpeed;
    public float Acceleration;
    public float StoppingDistance;
    public float DetectionRange;
    public float ViewAngle;

    public float AttackCooldown;
    public float AnticipationTime;      
    public float HitboxActiveDuration; 
    public float AttackDashSpeed;     
    public float AttackDashDuration;
    
    public float KoTime;
    public int MaxKo;
    public int CurrentKo;

    public float SearchDuration;
    public float SearchRadius;

    public int DamageToApply;
    public float StunDuration;

    public Sprite SpriteSearch;
    public Sprite SpriteAttackStart;
    public Sprite SpriteAttackEnd;
    public Sprite SpriteChase;
    public Sprite SpritePatrol;
    public Sprite SpriteKo;
    public Sprite SpriteFall;
    public Sprite SpriteTakeDamage;
    public Sprite SpriteExposed;
    
    public float ExposedTime;

    public EnemyDataInstance(EnemyData data)
    {
        PatrolSpeed = data.EnemyNavigationData.PatrolSpeed;
        ChaseSpeed = data.EnemyNavigationData.ChaseSpeed;
        StoppingDistance = data.EnemyNavigationData.StoppingDistance;
        DetectionRange = data.EnemyNavigationData.DetectionRange;
        ViewAngle = data.EnemyNavigationData.ViewAngle;
        Acceleration = data.EnemyNavigationData.Acceleration;

        AttackCooldown = data.EnemyAttackData.AttackCooldown;
        AnticipationTime = data.EnemyAttackData.AnticipationTime;
        HitboxActiveDuration = data.EnemyAttackData.HitboxActiveDuration;
        AttackDashSpeed = data.EnemyAttackData.AttackDashSpeed;
        AttackDashDuration = data.EnemyAttackData.AttackDashDuration;
        
        KoTime = data.EnemyKoData.KoTime;
        MaxKo = data.EnemyKoData.MaxKo;
        CurrentKo = 0;

        SearchDuration = data.EnemySearchData.searchDuration;
        SearchRadius = data.EnemySearchData.searchRadius;

        DamageToApply = data.EnemyTakeDamageData.DamageToApply;
        StunDuration = data.EnemyTakeDamageData.StunDuration;
        
        SpriteSearch = data.EnemyDataFeedBack.SpriteSearch;
        SpriteAttackStart = data.EnemyDataFeedBack.SpriteAttackStart;
        SpriteChase = data.EnemyDataFeedBack.SpriteChase;
        SpritePatrol =  data.EnemyDataFeedBack.SpritePatrol;
        SpriteKo =  data.EnemyDataFeedBack.SpriteKo;
        SpriteFall = data.EnemyDataFeedBack.SpriteFall;
        SpriteTakeDamage = data.EnemyDataFeedBack.SpriteTakeDamage;
        SpriteExposed = data.EnemyDataFeedBack.SpriteExposed;
        
        ExposedTime = data.EnemyExposedData.ExposedTime;
    }
    
    public bool IsKoFull() => CurrentKo >= MaxKo;
    
    public void ResetKo() => CurrentKo = 0;

    public Sprite GetSprite(EnemyBaseState currentBaseState)
    {
        switch (currentBaseState.Name)
        {
            case "Chase":
                return SpriteChase;
            case "Searching":
                return SpriteSearch;
            case "Attacking":
                return SpriteAttackStart; 
            case "Patrol":
                return SpritePatrol;
            case "K-O":
                return SpriteKo;
            case "Falling":
                return SpriteFall;
            case "Taking Damage":
                return SpriteTakeDamage;
            case "Exposed":
                return SpriteExposed;
            default:
                return null;
        }
    }
    
}

