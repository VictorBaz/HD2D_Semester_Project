using UnityEngine;

public class ParryHitbox : MonoBehaviour
{
    [SerializeField] private Transform transformEntity;
    [SerializeField] private float parryAngle = 0.5f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IDamageableEnemy target)) return;

        if (IsInFront(target.GetTransform()))
        {
            target.GettingParry();
        }
    }

    private bool IsInFront(Transform target)
    {
        Vector3 dirToTarget = (target.position - transformEntity.position).normalized;
        return Vector3.Dot(transformEntity.forward, dirToTarget) > parryAngle;
    }
}