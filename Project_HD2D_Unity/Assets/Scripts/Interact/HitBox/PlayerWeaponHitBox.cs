using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHitbox : BaseHitbox
{
    [Header("Weapon Specific")]
    [SerializeField] private int    damage    = 10;
    [SerializeField] private string targetTag = "Enemy";
    [SerializeField] private PlayerManager playerManager;

    private List<IDamageable> alreadyHitTargets = new();

    private void OnEnable() => alreadyHitTargets.Clear();

    private void OnTriggerEnter(Collider other)
    {
        if (!IsTarget(other)) return;
        
        var target = other.GetComponent<IDamageableEnemy>();
        if (target == null || alreadyHitTargets.Contains(target)) return;

        if (!HasClearLineTo(other)) return;

        target.TakeDamage(damage, transform.forward,playerManager.AttackState.ComboIndex);
        alreadyHitTargets.Add(target);
    }

    private bool IsTarget(Collider other)
    {
        return other.CompareTag(targetTag);
    }
}