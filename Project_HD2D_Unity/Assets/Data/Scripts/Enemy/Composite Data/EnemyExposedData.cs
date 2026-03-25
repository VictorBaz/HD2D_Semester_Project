using UnityEngine;

[CreateAssetMenu(fileName = "AiExposedData", menuName = "Enemy/AiExposedData")]
public class EnemyExposedData : ScriptableObject
{
    [field: SerializeField] public float ExposedTime { get; set; } = 1f;
}