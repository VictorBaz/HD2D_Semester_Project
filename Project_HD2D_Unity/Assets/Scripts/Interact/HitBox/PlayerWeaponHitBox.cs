using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHitbox : BaseHitbox
{
    [Header("Weapon Specific")]
    [SerializeField] private int    damage    = 10;
    [SerializeField] private string targetTag = "Enemy";

    private List<IDamageable> alreadyHitTargets = new();

    private void OnEnable() => alreadyHitTargets.Clear();

    private void OnTriggerEnter(Collider other)
    {
        if (!IsTarget(other)) return;

        var target = other.GetComponentInParent<IDamageable>();
        if (target == null || alreadyHitTargets.Contains(target)) return;

        if (!HasClearLineTo(other)) return;

        target.TakeDamage(damage, transform.forward);
        alreadyHitTargets.Add(target);
    }

    private bool IsTarget(Collider other)
    {
        return other.CompareTag(targetTag) ||
               (other.transform.parent != null && other.transform.parent.CompareTag(targetTag));
    }
}