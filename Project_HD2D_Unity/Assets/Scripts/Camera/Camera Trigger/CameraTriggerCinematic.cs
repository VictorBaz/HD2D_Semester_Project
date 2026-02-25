using Enum;
using Manager;
using UnityEngine;

public class CameraTriggerCinematic : CameraTriggerBase
{
    [SerializeField] private Vector3 cameraPosition;
    [SerializeField] private float holdDuration = 2f;

    private bool enter = false;
    
    protected override void Trigger()
    {
        bool isCinematic = !enter;
        enter = true;

        CameraSettings settings = new CameraSettings
        {
            CameraPosition = cameraPosition,
            CameraPlayerState = isCinematic ? CameraPlayerState.Cinematic : CameraPlayerState.FollowPlayer,
            holdDuration = isCinematic ? holdDuration : 0f
        };

        EventManager.TriggerCamera(settings);
    }
}