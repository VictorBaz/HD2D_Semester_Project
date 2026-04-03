using System.Collections.Generic;
using UnityEngine;

public class EnemyRepulsionHitbox : BaseHitbox
{
    [Header("Repulsion Settings")]
    [SerializeField] private float repulsionForce = 15f;
    [SerializeField] private string targetTag = "Player";

    private readonly Dictionary<Collider, Rigidbody> _rbCache = new();

    private void OnDisable()
    {
        _rbCache.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsTarget(other)) return;

        Rigidbody rb = other.GetComponentInParent<Rigidbody>();
        
        if (rb != null)
        {
            _rbCache[other] = rb;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _rbCache.Remove(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_rbCache.TryGetValue(other, out Rigidbody targetRb)) return;

        if (!HasClearLineTo(other)) return;

        Vector3 direction = other.transform.position - transform.position;
        direction.y = 0;

        if (direction == Vector3.zero) direction = transform.forward;
        direction.Normalize();

        targetRb.AddForce(direction * repulsionForce, ForceMode.VelocityChange);
    }

    private bool IsTarget(Collider other)
    {
        return other.CompareTag(targetTag) || 
               (other.transform.parent != null && other.transform.parent.CompareTag(targetTag));
    }
}