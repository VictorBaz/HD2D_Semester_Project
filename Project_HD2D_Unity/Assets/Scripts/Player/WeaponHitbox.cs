using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private string TagName = "";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(TagName) && !other.transform.parent.CompareTag(TagName)) return;

        var target = other.GetComponentInParent<IDamageable>();
    
        if (target != null)
        {
            target.TakeDamage(damage, transform.forward);
        }
    }
}