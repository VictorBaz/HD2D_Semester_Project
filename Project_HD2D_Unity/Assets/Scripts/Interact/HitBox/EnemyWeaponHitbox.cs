using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponHitbox : BaseHitbox
{
    [Header("Weapon Specific")]
    [SerializeField] private EnemyBaseManager manager;
    [SerializeField] private int    damage    = 10;
    [SerializeField] private string targetTag = "Player";

    private List<IDamageable> alreadyHitTargets = new();

    private void OnEnable() => alreadyHitTargets.Clear();

    private void OnTriggerEnter(Collider other)
    {
        if (!IsTarget(other)) return;

        var target = other.GetComponentInParent<IDamageable>();
        if (target == null || alreadyHitTargets.Contains(target)) return;

        if (!HasClearLineTo(other)) return;

        if (target.IsInParryWindowPerfect())
        {
            manager.HandlePerfectParry();
        }
        else if (target.IsInParryWindow())
        {
            manager.TakeDamage(damage, -transform.forward);
            alreadyHitTargets.Add(target);
        }
        else
        {
            target.TakeDamage(damage, transform.forward);
            alreadyHitTargets.Add(target);
        }
    }

    private bool IsTarget(Collider other)
    {
        return other.CompareTag(targetTag) ||
               (other.transform.parent != null && other.transform.parent.CompareTag(targetTag));
    }
}