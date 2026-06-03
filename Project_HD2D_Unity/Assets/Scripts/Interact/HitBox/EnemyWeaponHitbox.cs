using System.Collections.Generic;
using Script.Manager;
using UnityEngine;

public class EnemyWeaponHitbox : BaseHitbox
{
    [Header("Weapon Specific")]
    [SerializeField] private EnemyBaseManager manager;
    [SerializeField] private int    damage    = 10;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private int damageParry = 10;

    private List<IDamageable> alreadyHitTargets = new();

    private void OnEnable() => alreadyHitTargets.Clear();

    private void OnTriggerEnter(Collider other)
    {
        if (!IsTarget(other)) return;

        var target = other.GetComponentInParent<IDamageable>();
        if (target == null || alreadyHitTargets.Contains(target)) return;

        if (!HasClearLineTo(other)) return;

        if (target.IsInParryWindowPerfect() || target.IsInParryWindow())
        {
            SoundManager.Instance?.PlaySfx(SoundType.Parry_Perfect);

            if (target is PlayerManager player)
            {
                player.Context?.VfxManagerPlayer.TriggerParryDone();
            }
            
            manager.TakeDamage(damage,Vector3.zero);
        }
        else
        {
            target.TakeDamage(damageParry, transform.forward);
            alreadyHitTargets.Add(target);
        }
    }

    private bool IsTarget(Collider other)
    {
        return other.CompareTag(targetTag) ||
               (other.transform.parent != null && other.transform.parent.CompareTag(targetTag));
    }
}