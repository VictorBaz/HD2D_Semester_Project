using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected float m_speed = 10f;
    [SerializeField] private int m_damage = 1;


    [SerializeField] protected Rigidbody m_rb;

    [SerializeField] protected string poolKey = "Projectile_";
    public string PoolKey => poolKey;
    

    public virtual void Initialize(Vector2 direction)
    {
        Vector3 dir3D = new Vector3(direction.x, 0f, direction.y);
        m_rb.linearVelocity = dir3D * m_speed;

        if (dir3D != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir3D);
    }
    
    public virtual void Initialize(Transform from, Transform to)
    {
        StartCoroutine(InitializeIe(from, to));
    }

    private IEnumerator InitializeIe(Transform from, Transform to)
    {
        Vector3 startPos  = from.position;
        Vector3 targetPos = to.position;
        float elapsed  = 0f;
        float duration = 1f;

        Vector3 midPoint = (startPos + targetPos) / 2f + Vector3.up * 3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            Vector3 a = Vector3.Lerp(startPos, midPoint,  t);
            Vector3 b = Vector3.Lerp(midPoint, targetPos, t);
            transform.position = Vector3.Lerp(a, b, t);

            Vector3 dir = (b - a).normalized;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);

            yield return null;
        }

        transform.position = targetPos;
        ImpactBehaviour();
    }

    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // TODO: retrieve IDamageable from other and apply m_damage
        }

        ImpactBehaviour();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // TODO: retrieve IDamageable from other and apply m_damage
        }

        ImpactBehaviour();
    }
    
    public virtual void ImpactBehaviour()
    {

        m_rb.linearVelocity = Vector3.zero;

        ObjectPooler.EnqueueObject(this, PoolKey);
    }
}
