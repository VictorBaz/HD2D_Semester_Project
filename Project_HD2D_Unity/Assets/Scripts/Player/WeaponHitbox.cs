using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") || !other.TryGetComponent<IDamageable>(out var target)) return;
        
        target.TakeDamage(damage);
        print("oulalala");
    }
}