using System.Collections;
using UnityEngine;

public class CameraCinematicState : CameraBaseState
{
    private float timer;
    private bool isHolding;
    private bool transition = false;
    private Vector3 targetPosition;

    public override void EnterState(CameraStateContext context)
    {
        // 1. SI c'est les crédits, on calcule d'abord la logique des crédits 
        // pour surcharger le timer de la caméra avec le bon timing global.
        if (context.CurrentSettings.isCredit)
        {
            CreditsLogic(context);
        }
        else
        {
            timer = context.CurrentSettings.holdDuration;
        }

        isHolding = false;
        transition = false;

        targetPosition = context.CurrentSettings.targetCinematic ? 
            context.CurrentSettings.targetCinematic.position : context.PlayerTransform.position;
    }

    public override void UpdateState(CameraStateContext context)
    {
        if (transition) return;
        
        context.CameraTransform.position = Vector3.SmoothDamp(
            context.CameraTransform.position,
            context.CurrentSettings.CameraTargetTransform.position,
            ref context.Velocity,
            context.TransitionSpeed 
        );

        ApplyRestrictedRotation(context, targetPosition);

        float sqrDistance = (context.CameraTransform.position - context.CurrentSettings.CameraTargetTransform.position).sqrMagnitude;
        
        if (!isHolding && sqrDistance < 0.01f) isHolding = true;

        if (isHolding)
        {
            timer -= Time.deltaTime;
            
            if (timer <= 0)
            {
                transition = true;
                context.Manager.ReturnFromCinematic();
            }
        }
    }

    public override void ExitState(CameraStateContext context) 
    {
        isHolding = false;
    }

    private void CreditsLogic(CameraStateContext context)
    {
        float scrollDuration = 20f;
        
        timer = context.CurrentSettings.holdDuration + scrollDuration;
        
        GameplayEvents.TriggerCredits(scrollDuration);
    }
}