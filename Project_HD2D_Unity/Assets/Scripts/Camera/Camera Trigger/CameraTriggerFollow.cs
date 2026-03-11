using Enum;
using Manager;
using UnityEngine;

public class CameraTriggerFollow : CameraTriggerBase
{
    protected override void Trigger()
    {
        CameraSettings settings = new CameraSettings
        {
            CameraPlayerState = CameraPlayerState.FollowPlayer
        };
        
        EventManager.TriggerCamera(settings);
    }
}