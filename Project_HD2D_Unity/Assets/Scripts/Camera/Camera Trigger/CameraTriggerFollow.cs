using System;
using Enum;
using Manager;
using UnityEngine;

public class CameraTriggerFollow : CameraTriggerBase
{
    protected override Color GizmoColor => new Color(0, 1, 0, 0.2f);
    
    protected override string Name => "Follow Camera Gizmo";
    
    protected override void Trigger()
    {
        CameraSettings settings = new CameraSettings
        {
            CameraPlayerState = CameraPlayerState.FollowPlayer
        };
        
        EventManager.TriggerCamera(settings);
    }

    
}