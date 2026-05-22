using Enum;
using UnityEngine;

public class CameraTriggerCinematic : CameraTriggerBase
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float holdDuration = 2f;

    private bool enter = false;
    
    protected override Color GizmoColor => new Color(0, 0.5f, 1, 0.2f);
    
    protected override string Name => "Cinematic Camera Gizmo";
    
    protected override void Trigger()
    {
        bool isCinematic = !enter;
        enter = true;

        CameraSettings settings = new CameraSettings
        {
            CameraTargetTransform = cameraTransform,
            CameraPlayerState = isCinematic ? CameraPlayerState.Cinematic : CameraPlayerState.FollowPlayer,
            holdDuration = isCinematic ? holdDuration : 0f
        };

        CameraEvents.TriggerCamera(settings);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.darkBlue;
        Gizmos.DrawLine(transform.position, cameraTransform.position);
        Gizmos.DrawSphere(cameraTransform.position, 1f);
    }
}