using UnityEngine;
using UnityEngine.AI;

public class SheepBridgeState : EnemyBaseState
{
    private EnemySheepManager sheepManager;
    private float timer;
    private float duration;
    private SheepAnimationManager sheepAnimationManager;

    private const float minFrequency = 1.5f;
    private const float maxFrequency = 25f;
    private const float maxIntensity = 5f; 
    
    private SteamAchivementManager ACHManager;
    private string achievementId = "ACH_SHEEP_IN_WATER";

    public override void EnterState(EnemyContext context)
    {
        ACHManager = SteamAchivementManager.Instance;
        
        if (ACHManager == null)
        {
            Debug.LogWarning("Steam manager is NULL !");
        }
        else
        {
            ACH_Unlock();
        }
        
        ACHManager = SteamAchivementManager.Instance;
        
        if (ACHManager == null)
        {
            Debug.LogError("Steam manager is NULL !");
        }
        else
        {
            ACH_Unlock();
        }
        
        context.VfxManager.StopKoVfx();
        
        if (!sheepManager) sheepManager = context.Manager as EnemySheepManager;
        if (!sheepAnimationManager) sheepAnimationManager = context.AnimManager as SheepAnimationManager;
        
        if (sheepManager)
        {
            duration = 10f; 
            sheepManager.ActivateBridge();
        }

        timer = 0f;

        context.Manager.ApplyMovementMode(true);
        context.Rb.isKinematic = true;

        if (sheepAnimationManager)
        {
            sheepAnimationManager.SetBridgeOn();
        }
        
        context.AnimManager.ToggleRepulsiveCollider(false);
    }

    public override void UpdateState(EnemyContext context)
    {
        UpdateTimer(context);
        ApplyBlinkEffect(context);

        if (!Physics.Raycast(context.Manager.transform.position,
                Vector3.down,
                context.Data.GroundCheckDistance,
                1 << LayerMask.NameToLayer("Water"),
                QueryTriggerInteraction.Collide))
        {
            context.TransitionTo(context.Manager.DropState);
        }
    }

    private void UpdateTimer(EnemyContext context)
    {
        timer += Time.deltaTime;

        if (timer >= duration)
        {
            HandleTimeOut(context);
        }
    }

    private void ApplyBlinkEffect(EnemyContext context)
    {
        float progress = timer / duration; 

        float currentFrequency = Mathf.Lerp(minFrequency, maxFrequency, progress * progress);
        
        float pulse = Mathf.Sin(timer * currentFrequency);

        float intensity = ((pulse + 1f) * 0.5f) * maxIntensity;

        context.SetVisualParam(GameConstants.PARAM_SHEEP_SHADER_NAME, intensity, GameConstants.INDEX_MATERIAL_PULSE);
    }

    private void HandleTimeOut(EnemyContext context)
    {
        context.SetVisualParam(GameConstants.PARAM_SHEEP_SHADER_NAME, 0f, GameConstants.INDEX_MATERIAL_PULSE);
        
        sheepManager?.ResetEnemy();
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

    public override void ExitState(EnemyContext context)
    {
        sheepManager?.DeactivateBridge();
        sheepAnimationManager?.SetBridgeOff();
        context.AnimManager.ToggleRepulsiveCollider(true);
    }
}