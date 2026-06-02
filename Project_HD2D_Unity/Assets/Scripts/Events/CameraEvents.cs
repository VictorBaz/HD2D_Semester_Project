using System;
using UnityEngine;

public static class CameraEvents
{
    public static event Action<CameraSettings> OnCameraTrigger;
    public static event Action OnCameraShake;

    public static void TriggerCamera(CameraSettings settings) => OnCameraTrigger?.Invoke(settings);
    public static void CameraShake() => OnCameraShake?.Invoke();
    
    public static Func<Transform> RequestGetCameraTransform;
}