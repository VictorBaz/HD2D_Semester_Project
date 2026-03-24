using UnityEngine;

[CreateAssetMenu(fileName = "JumpData", menuName = "Player/Data/JumpData")]
public class JumpData : ScriptableObject
{
    [field: Header("Jump Settings")]
    [field: SerializeField] public float JumpCooldown { get; private set; } = 0.2f;
    
    [field: SerializeField] public float JumpHeight { get; private set; } = 2.0f;
    [field: SerializeField] public float JumpDuration { get; private set; } = 0.4f;
    
    [field: Header("In Air Settings")]
    [field: SerializeField] public float GravityMultiplier { get; private set; } = 3f;
    [field: SerializeField] public float MaxGravityTime { get; private set; } = 0.8f;
}