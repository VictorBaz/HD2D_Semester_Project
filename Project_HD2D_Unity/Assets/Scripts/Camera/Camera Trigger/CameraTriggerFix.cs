using Enum;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CanEditMultipleObjects]
public class CameraTriggerFix : CameraTriggerBase
{
    [SerializeField] private Transform targetTransform;
    [FormerlySerializedAs("cameraPosition")] [SerializeField] private Vector3 cameraPositionUnused;

    protected override Color GizmoColor => new Color(1, 0, 0, 0.2f);
    
    protected override string Name => "Fix Camera Gizmo";
    
    protected override void Trigger()
    {
        CameraSettings settings = new CameraSettings
        {
            CameraPosition = targetTransform.position,
            CameraPlayerState = CameraPlayerState.Fix
        };
        
        CameraEvents.TriggerCamera(settings);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!targetTransform) return;
        
        Gizmos.color = Color.darkRed;
        Gizmos.DrawLine(transform.position, targetTransform.position);
        Gizmos.DrawSphere(targetTransform.position, 1f);
    }
}