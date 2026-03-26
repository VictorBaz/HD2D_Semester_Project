using UnityEngine;

[CreateAssetMenu(fileName = "AiTakeDamageData", menuName = "Enemy/AiTakeDamageData")]
public class EnemyTakeDamageData : ScriptableObject
{
   [field:SerializeField] public int DamageToApply { get; private set; } = 1;
   [field:SerializeField] public float StunDuration { get; private set; } = 0.2f;
}