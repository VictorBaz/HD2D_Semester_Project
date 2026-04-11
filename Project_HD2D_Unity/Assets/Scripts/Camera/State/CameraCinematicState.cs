using UnityEngine;

public class CameraCinematicState : CameraBaseState
{
    private float timer;
    private bool isHolding;

    public override void EnterState(CameraStateContext context)
    {
        timer = context.CurrentSettings.holdDuration;
        isHolding = false;
    }

    public override void UpdateState(CameraStateContext context)
    {
        context.CameraTransform.position = Vector3.SmoothDamp(
            context.CameraTransform.position,
            context.CurrentSettings.CameraPosition,
            ref context.Velocity,
            context.Manager.TravelDuration 
        );

        ApplyRestrictedRotation(context, context.PlayerTransform.position + Vector3.up * 1.5f);

        float distance = Vector3.Distance(context.CameraTransform.position, context.CurrentSettings.CameraPosition);
        
        if (distance < 0.1f) isHolding = true;

        if (isHolding)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                context.Manager.TransitionTo(context.Manager.FollowState);
            }
        }
    }

    public override void ExitState(CameraStateContext context) 
    {
        isHolding = false;
    }
}