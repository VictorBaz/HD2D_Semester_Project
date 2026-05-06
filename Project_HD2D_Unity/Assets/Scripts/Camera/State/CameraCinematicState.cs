using UnityEngine;

public class CameraCinematicState : CameraBaseState
{
    private float timer;
    private bool isHolding;
    private bool transition = false;

    public override void EnterState(CameraStateContext context)
    {
        timer = context.CurrentSettings.holdDuration;
        isHolding = false;
        transition = false;
    }

    public override void UpdateState(CameraStateContext context)
    {
        if (transition) return;
        
        context.CameraTransform.position = Vector3.SmoothDamp(
            context.CameraTransform.position,
            context.CurrentSettings.CameraPosition,
            ref context.Velocity,
            context.TransitionSpeed 
        );

        ApplyRestrictedRotation(context, context.CurrentSettings.targetCinematic.position);

        float sqrDistance = (context.CameraTransform.position - context.CurrentSettings.CameraPosition).sqrMagnitude;
        
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
}