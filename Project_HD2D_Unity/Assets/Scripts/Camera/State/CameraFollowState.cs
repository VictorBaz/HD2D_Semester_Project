using UnityEngine;

public class CameraFollowState : CameraBaseState
{
    public override void EnterState(CameraStateContext context) { }

    public override void UpdateState(CameraStateContext context)
    {
        Vector3 targetPosition = context.PlayerTransform.position + context.Offset;
        
        context.CameraTransform.position = Vector3.SmoothDamp(
            context.CameraTransform.position,
            targetPosition,
            ref context.Velocity,
            context.SmoothTimeFollow
        );
    }

    public override void ExitState(CameraStateContext context) { }
}