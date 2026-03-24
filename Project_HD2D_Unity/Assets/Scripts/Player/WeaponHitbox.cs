using UnityEngine;
using System.Collections.Generic;

public class WeaponHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private string targetTag = "Enemy";

    private List<IDamageable> alreadyHitTargets = new List<IDamageable>();

    private void OnEnable()
    {
        alreadyHitTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag) && (other.transform.parent == null || !other.transform.parent.CompareTag(targetTag))) 
            return;

        var target = other.GetComponentInParent<IDamageable>();

        if (target == null) return;
        
        if (alreadyHitTargets.Contains(target)) return;
        
        target.TakeDamage(damage, transform.forward);
        alreadyHitTargets.Add(target);
    }
}