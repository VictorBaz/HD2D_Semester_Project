using UnityEngine;

[CreateAssetMenu(fileName = "JumpData", menuName = "Player/Data/JumpData")]
public class JumpData : ScriptableObject
{
    [field: Header("Jump Settings")]
    
    [field: SerializeField] public float JumpForce { get; private set; } = 5f;
    [field: SerializeField] public float JumpCooldown { get; private set; } = 0.2f;
    
    [field: Header("In Air Settings")]
    [field: SerializeField] public float MaxVerticalVelocity { get; private set; } = 12f;
    [field: SerializeField] public float GravityMultiplier { get; private set; } = 3f;
    [field: SerializeField] public float MaxGravityTime { get; private set; } = 0.8f;
    [field: SerializeField] public float JumpCutMultiplier { get; private set; } = 0.4f;
}