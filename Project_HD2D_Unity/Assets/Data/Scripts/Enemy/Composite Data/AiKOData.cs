using UnityEngine;

[CreateAssetMenu(fileName = "AiKOData", menuName = "Enemy/AiKOData")]
public class AiKOData : ScriptableObject
{
    [field: SerializeField] public float KoTime { get; private set; } = 15f;
}