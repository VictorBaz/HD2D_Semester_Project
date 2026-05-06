using UnityEngine;

public class CameraFixState : CameraBaseState
{
    public override void EnterState(CameraStateContext context) { }
    public override void UpdateState(CameraStateContext context)
    {
        if (context.CurrentSettings == null)
        {
            Debug.LogError("No camera settings found.");
            return;
        }
        
        context.CameraTransform.position = Vector3.SmoothDamp(
            context.CameraTransform.position,
            context.CurrentSettings.CameraPosition,
            ref context.Velocity,
            context.TransitionSpeed
        );
        
        ApplyRestrictedRotation(context, context.PlayerTransform.position + Vector3.up * 1.5f);
    }

    public override void ExitState(CameraStateContext context) { }
}