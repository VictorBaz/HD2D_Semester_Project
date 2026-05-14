using UnityEngine;

public interface IDamageableEnemy : IDamageable
{
        void TakeDamageIndex(int value, Vector3 hitDirection,int index);
}