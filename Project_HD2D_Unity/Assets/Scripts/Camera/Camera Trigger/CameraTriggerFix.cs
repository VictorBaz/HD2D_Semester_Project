using Enum;
using Manager;
using UnityEngine;

public class CameraTriggerFix : CameraTriggerBase
{
    [SerializeField] private Vector3 cameraPosition;
    
    protected override void Trigger()
    {
        CameraSettings settings = new CameraSettings
        {
            CameraPosition = cameraPosition,
            CameraPlayerState = CameraPlayerState.Fix
        };
        
        EventManager.TriggerCamera(settings);
    }
}