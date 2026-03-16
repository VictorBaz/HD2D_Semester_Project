using UnityEngine;

[CreateAssetMenu(fileName = "AiSearchData", menuName = "Enemy/AiSearchData")]
public class AiSearchData : ScriptableObject
{
    [field:SerializeField] public float searchDuration { get; private set; } = 10f;
    [field:SerializeField] public float searchRadius { get; private set; } = 5f;
}