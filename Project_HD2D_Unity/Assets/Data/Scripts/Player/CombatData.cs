using UnityEngine;

[CreateAssetMenu(fileName = "CombatData", menuName = "Player/Data/CombatData")]
public class CombatData : ScriptableObject
{

    [field: Header("Combo")]
    [field: Tooltip("Time window after an attack during which a combo input is accepted")]
    [field: SerializeField] public float ComboWindow { get; private set; } = 0.5f;
    [field: SerializeField] public CombatHitData[] ComboHits { get; private set; }

    
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