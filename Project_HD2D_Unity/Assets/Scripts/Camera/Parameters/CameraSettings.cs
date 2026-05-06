using UnityEngine;

[System.Serializable]
public class CameraSettings
{
    public Enum.CameraPlayerState CameraPlayerState;
    public Transform CameraTargetTransform;
    public float holdDuration;
    public Rail ActiveRail; 
    
    [Header("Transition")]
    [Tooltip("Vitesse de transition vers cet état (SmoothTime)")]
    [Range(0.01f, 2f)] public float transitionSmoothTime = 0.3f; 
    
    [Header("Target Cinematic")]
    public Transform targetCinematic;
}