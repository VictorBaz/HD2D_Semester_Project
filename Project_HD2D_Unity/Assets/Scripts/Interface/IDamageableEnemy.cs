using UnityEngine;

public interface IDamageableEnemy : IDamageable
{
        void TakeDamage(int value, Vector3 hitDirection,int index);
}