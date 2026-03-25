using UnityEngine;

[CreateAssetMenu(fileName = "PlayerResourcesData", menuName = "Player/Data/PlayerResourcesData")]
public class PlayerResourcesData : ScriptableObject
{
    [field: Header("ENERGY"),SerializeField] public int Energy { get; private set; }
    [field: SerializeField] public int MaxEnergy { get; private set; }
        
    [field:Header("LIFE"), SerializeField] public int Life { get; private set; }
    [field: SerializeField] public int MaxLife { get; private set; }
}