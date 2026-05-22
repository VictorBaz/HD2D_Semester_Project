using UnityEngine;

public class CameraRailState : CameraBaseState
{
    private Rail currentRail;

    public override void EnterState(CameraStateContext context)
    {
        if (context.CurrentSettings != null)
        {
            currentRail = context.CurrentSettings.ActiveRail;
        }
    }

    public override void UpdateState(CameraStateContext context)
    {
        Rail activeRail = context.CurrentSettings.ActiveRail;

        if (!activeRail) return;

        Vector3 targetOnRail = activeRail.ProjectPositionOnRail(context.PlayerTransform.position);

        Vector3 finalPosition = targetOnRail + context.CurrentSettings.RailOffset;

        context.CameraTransform.position = Vector3.SmoothDamp(
            context.CameraTransform.position,
            finalPosition,
            ref context.Velocity,
            context.TransitionSpeed
        );

        ApplyRestrictedRotation(context, context.PlayerTransform.position + Vector3.up * 1.5f);
    }

    public override void ExitState(CameraStateContext context)
    {
        currentRail = null;
    }
}