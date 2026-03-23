using UnityEngine;

[CreateAssetMenu(fileName = "CombatData", menuName = "Player/Data/CombatData")]
public class CombatData : ScriptableObject
{

    [field: Header("Combo")]
    [field: SerializeField] public CombatHitData[] ComboHits { get; private set; }
    [field: Tooltip("Time window after an attack during which a combo input is accepted")]
    [field: SerializeField] public float ComboWindow { get; private set; } = 0.5f;

    
}

[System.Serializable]
public class CombatHitData
{
    public float         DashSpeed;
    public float         DashDuration;
    public AnimationClip Clip;
}