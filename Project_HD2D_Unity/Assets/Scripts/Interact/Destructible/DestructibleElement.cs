using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class DestructibleElement : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject intactVisual;
    [SerializeField] private GameObject fracturedParent;
    [SerializeField] private ParticleSystem breakParticles;

    [Header("Settings")]
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 2f;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onDestructionEvent;

    private bool isDestroyed = false;

    private void Start()
    {
        if (intactVisual) intactVisual.SetActive(true);
        if (fracturedParent) fracturedParent.SetActive(false);
    }

    private void TriggerDestruction()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        if (breakParticles) breakParticles.Play();

        if (intactVisual) intactVisual.SetActive(false);

        if (fracturedParent)
        {
            fracturedParent.SetActive(true);
            DestructionHelper.Explode(fracturedParent, transform.position, explosionForce, explosionRadius);
        }

        onDestructionEvent?.Invoke();

    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return;
        if (!collision.gameObject.TryGetComponent<EnemyBaseManager>(out var enemy)) return;
        
        if (enemy.CurrentState is EnemyDropState)
        {
            TriggerDestruction();
        }
    }
}