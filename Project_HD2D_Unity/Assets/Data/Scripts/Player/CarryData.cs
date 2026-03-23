using UnityEngine;

[CreateAssetMenu(fileName = "CarryData", menuName = "Player/Data/CarryData")]
public class CarryData : ScriptableObject
{
    [field: SerializeField] public float CarryRange { get; private set; } = 15f;
    [field: SerializeField] public float CarryAngle { get; private set; } = 90f;
    [field: SerializeField] public LayerMask CarryLayer { get; private set; }
}