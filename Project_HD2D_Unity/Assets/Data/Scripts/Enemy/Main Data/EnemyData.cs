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
    public float KnockbackStrength;
    public float AnticipationTime;

    public float KoTime;

    public float SearchDuration;
    public float SearchRadius;

    public int DamageToApply;
    public float StunDuration;

    public EnemyDataInstance(EnemyData data)
    {
        AttackCooldown = data.AiAttackData.AttackCooldown;
        KnockbackStrength = data.AiAttackData.KnockbackStrength;
        AnticipationTime = data.AiAttackData.AnticipationTime;

        KoTime = data.AiKOData.KoTime;

        SearchDuration = data.AiSearchData.searchDuration;
        SearchRadius = data.AiSearchData.searchRadius;

        DamageToApply = data.AiTakeDamageData.DamageToApply;
        StunDuration = data.AiTakeDamageData.StunDuration;
    }
}