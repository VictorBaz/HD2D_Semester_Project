using Enum;
using Manager;
using UnityEngine;

public class CameraTriggerFix : CameraTriggerBase
{
    [SerializeField] private Vector3 cameraPosition;
    
    protected override Color GizmoColor => new Color(1, 0, 0, 0.2f);
    
    protected override string Name => "Fix Camera Gizmo";
    
    protected override void Trigger()
    {
        CameraSettings settings = new CameraSettings
        {
            CameraPosition = cameraPosition,
            CameraPlayerState = CameraPlayerState.Fix
        };
        
        EventManager.TriggerCamera(settings);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.darkRed;
        Gizmos.DrawLine(transform.position, cameraPosition);
        Gizmos.DrawSphere(cameraPosition, 1f);
    }
}