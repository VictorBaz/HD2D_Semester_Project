using UnityEngine;

[System.Serializable]
public class CameraSettings
{
    public Enum.CameraPlayerState CameraPlayerState;
    public Vector3 CameraPosition;
    public float holdDuration;
    public Rail ActiveRail; 
    
    [Header("Transition")]
    [Tooltip("Vitesse de transition vers cet état (SmoothTime)")]
    [Range(0.01f, 2f)] public float transitionSmoothTime = 0.3f; 
    
    [Header("Rotation Constraints")]
    public float lockedZRotation = 0f;
    
    [Header("Target Cinematic")]
    public Transform targetCinematic;
}