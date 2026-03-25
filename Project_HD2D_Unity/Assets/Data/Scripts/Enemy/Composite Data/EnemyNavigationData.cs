using UnityEngine;

[CreateAssetMenu(fileName = "AiNavigationData", menuName = "Enemy/AiNavigationData")]
public class EnemyNavigationData : ScriptableObject
{
    [Header("Speeds")]
    [field:SerializeField] public float PatrolSpeed { get; private set; } = 2f;
    [field:SerializeField] public float ChaseSpeed { get; private set; } = 4.5f;
    [field:SerializeField] public float Acceleration { get; private set; } = 8f;
    [field:SerializeField] public float StoppingDistance { get; private set; } = 1.2f;

    [Header("Detection")]
    [field:SerializeField] public float DetectionRange { get; private set; } = 15f;
    [field:SerializeField] public float ViewAngle { get; private set; } = 90f;
}