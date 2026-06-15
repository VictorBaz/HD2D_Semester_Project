using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class DestructibleElement : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject intactVisual;
    [SerializeField] private GameObject fracturedParent;
    [SerializeField] private ParticleSystem breakParticles;
    [SerializeField] private Renderer[] rendererTargets;
    
    [Header("Settings")]
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float dissolutionDuration = 1.0f;
    
    [Header("Achievement")]
    private SteamAchivementManager ACHManager;
    private string achievementId = "ACH_WALLBREAK";
    
    [Header("Events")]
    [SerializeField] private UnityEvent onDestructionEvent;

    private bool isDestroyed = false;
    
    private MaterialPropertyBlock block;
    private static readonly int ProgressionHash = Shader.PropertyToID("_Progression");

    private void Start()
    {
        if (intactVisual) intactVisual.SetActive(true);
        if (fracturedParent) fracturedParent.SetActive(false);
        block = new MaterialPropertyBlock();
        
        if (ACHManager != null)
            ACHManager = SteamAchivementManager.Instance;
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
        
        ACH_Unlock();
        
        GamepadVibrationHelper.Vibrate(0.15f,0.5f,0.25f);
        
        StartCoroutine(UpdateMpIe(dissolutionDuration));
        
        Destroy(gameObject, dissolutionDuration);
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return;
        if (!collision.gameObject.TryGetComponent<EnemyBaseManager>(out var enemy)) return;
        if (enemy.CurrentState is EnemyDropState) TriggerDestruction();
    }
    
    void ACH_Unlock()
    {
        if (ACHManager == null)
            return;
        
        if (!ACHManager.IsThisAchievementUnlocked(achievementId))
        {
            ACHManager.UnlockAchivement(achievementId);
        }
    }

    private IEnumerator UpdateMpIe(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            foreach (var rd in rendererTargets)
            {
                if (rd == null) continue; 
                
                rd.GetPropertyBlock(block);
                block.SetFloat(ProgressionHash, t); 
                rd.SetPropertyBlock(block);
            }
            
            yield return null;
        }
    }
}