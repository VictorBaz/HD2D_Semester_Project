using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [field: SerializeField] public AiAttackData AiAttackData { get; private set; }
    [field: SerializeField] public AiKOData AiKOData { get; private set; }
    [field: SerializeField] public AiSearchData AiSearchData { get; private set; }
    [field: SerializeField] public AiTakeDamageData AiTakeDamageData { get; private set; }
    
    
    public EnemyDataInstance Init() => new EnemyDataInstance(this);
}

public class EnemyDataInstance
{
    public float AttackCooldown;
    public float AnticipationTime;      
    public float HitboxActiveDuration; 
    public float AttackDashSpeed;     
    public float AttackDashDuration;
    
    public float KoTime;
    public int MaxKo;
    public int MinKo;
    public int CurrentKo;

    public float SearchDuration;
    public float SearchRadius;

    public int DamageToApply;
    public float StunDuration;

    public EnemyDataInstance(EnemyData data)
    {
        AttackCooldown = data.AiAttackData.AttackCooldown;
        AnticipationTime = data.AiAttackData.AnticipationTime;
        HitboxActiveDuration = data.AiAttackData.HitboxActiveDuration;
        AttackDashSpeed = data.AiAttackData.AttackDashSpeed;
        AttackDashDuration = data.AiAttackData.AttackDashDuration;
        
        
        KoTime = data.AiKOData.KoTime;
        MaxKo = data.AiKOData.MaxKo;
        MinKo = data.AiKOData.MinKo;
        CurrentKo = data.AiKOData.CurrentKo;

        SearchDuration = data.AiSearchData.searchDuration;
        SearchRadius = data.AiSearchData.searchRadius;

        DamageToApply = data.AiTakeDamageData.DamageToApply;
        StunDuration = data.AiTakeDamageData.StunDuration;
    }

    public bool IsKoFull()
    {
        return CurrentKo >= MaxKo;
    }
}