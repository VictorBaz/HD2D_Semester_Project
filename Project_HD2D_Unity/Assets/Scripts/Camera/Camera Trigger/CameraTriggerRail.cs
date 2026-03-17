using UnityEngine;

public class CameraTriggerRail : CameraTriggerBase
{
    [SerializeField] private Rail railToUse;

    protected override Color GizmoColor => new Color(1, 0, 1, 0.2f);
    
    protected override string Name => "Rail Camera Gizmo";
    
    protected override void Trigger()
    {
        CameraSettings settings = new CameraSettings
        {
            CameraPlayerState = Enum.CameraPlayerState.Rail,
            ActiveRail = railToUse
        };

        Manager.EventManager.TriggerCamera(settings);
    }
}