using UnityEngine;

public abstract class BaseHitbox : MonoBehaviour
{
    [Header("Base Hitbox Settings")]
    [SerializeField] protected LayerMask  obstacleLayer;
    [SerializeField] protected Transform  originTransform;

    private void Awake()
    {
        if (originTransform == null) originTransform = transform;
    }

    protected bool HasClearLineTo(Collider targetCollider)
    {
        Vector3 origin      = originTransform.position + Vector3.up;
        Vector3 targetPoint = targetCollider.bounds.center;
        Vector3 direction   = targetPoint - origin;
        float   distance    = direction.magnitude;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, obstacleLayer, QueryTriggerInteraction.Ignore))
        {
            return hit.collider != targetCollider;
        }

        return true;
    }
}