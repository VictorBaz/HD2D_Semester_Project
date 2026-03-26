using UnityEngine;

[CreateAssetMenu(fileName = "AiDataFeedBack", menuName = "Enemy/AiDataFeedBack")]
public class EnemyDataFeedBack : ScriptableObject
{
    [field: SerializeField] public Sprite SpriteSearch { get; set; }
    [field: SerializeField] public Sprite SpriteAttackStart { get; set; }
    [field: SerializeField] public Sprite SpriteChase { get; set; }
    [field: SerializeField] public Sprite SpritePatrol { get; set; }
    [field: SerializeField] public Sprite SpriteKo { get; set; }
    [field: SerializeField] public Sprite SpriteFall { get; set; }
    [field: SerializeField] public Sprite SpriteTakeDamage { get; set; }
    [field: SerializeField] public Sprite SpriteExposed { get; set; }
}