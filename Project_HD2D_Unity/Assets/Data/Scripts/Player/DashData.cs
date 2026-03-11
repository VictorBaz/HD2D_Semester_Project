using UnityEngine;

[CreateAssetMenu(fileName = "DashData", menuName = "Player/Data/DashData")]
public class DashData : ScriptableObject
{
    [field: Header("Dash")]
    [field: SerializeField] public float DashSpeed { get; private set; } = 20f;
    [field: SerializeField] public float DashDuration { get; private set; } = 0.4f;
    [field: SerializeField] public float DashCooldown { get; private set; } = 0.4f;
}