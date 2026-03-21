using UnityEngine;

[CreateAssetMenu(fileName = "AiKOData", menuName = "Enemy/AiKOData")]
public class AiKOData : ScriptableObject
{
    [field: SerializeField] public int MaxKo { get; private set; } = 100;
    [field: SerializeField] public int MinKo { get; private set; } = 0;
    
    [field: SerializeField] public float KoTime { get; private set; } = 15f;
    [field: SerializeField] public int CurrentKo { get; private set; } = 0;

}