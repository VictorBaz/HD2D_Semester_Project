using UnityEngine;

public class CameraFollowState : CameraBaseState
{
    public override void EnterState(CameraStateContext context) { }

    public override void UpdateState(CameraStateContext context)
    {
        Vector3 desiredPosition = context.PlayerTransform.position + context.Offset;

        Vector3 rayOrigin = context.PlayerTransform.position + new Vector3(0, 1f, 0);
        
        Vector3 finalPosition = CalculateCollision(desiredPosition, rayOrigin,context.CollisionPadding,context.CollisionLayers);

        context.CameraTransform.position = Vector3.SmoothDamp(
            context.CameraTransform.position, 
            finalPosition, 
            ref context.Velocity, 
            context.SmoothTimeFollow
        );
    }

    public override void ExitState(CameraStateContext context) { }
}