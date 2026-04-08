using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int value,Vector3 hitDirection);

    Transform GetTransform();

    bool IsInParryWindow();
    bool IsInParryWindowPerfect();

}