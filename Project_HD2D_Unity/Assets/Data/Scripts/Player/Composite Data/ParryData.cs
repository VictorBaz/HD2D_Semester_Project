using UnityEngine;

[CreateAssetMenu(fileName = "ParryData", menuName = "Player/Data/ParryData")]
public class ParryData : ScriptableObject
{
    [field: Header("Animation")]
    [field: SerializeField] public AnimationClip ParryAnimationClip { get; private set; }

    [field: Header("Timings de la Fenêtre de Parade")]

    [field: SerializeField] public float ParryHitboxStartOffset { get; private set; } = 0.1f;
    
    [field: SerializeField] public float ParryActiveDuration { get; private set; } = 0.2f;

    [field: Header("Paramètres de succès")]
    [field: SerializeField] public float HitStopDuration { get; private set; } = 0.1f;
}