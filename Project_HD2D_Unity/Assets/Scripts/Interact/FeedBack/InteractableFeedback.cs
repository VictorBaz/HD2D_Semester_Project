using UnityEngine;

[System.Serializable] 
public class FeedbackLogic
{
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float minDistance = 2f;

    private Transform _playerTransform;

    public void Initialize(Transform player)
    {
        _playerTransform = player;
    }

    public float CalculateAlpha(Vector3 ownerPosition)
    {
        if (_playerTransform == null) return 0f;

        float distance = Vector3.Distance(ownerPosition, _playerTransform.position);
        
        if (distance <= minDistance) return 1f;
        if (distance >= maxDistance) return 0f;

        return 1f - ((distance - minDistance) / (maxDistance - minDistance));
    }
    
}