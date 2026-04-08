using UnityEngine;

public class CameraStateContext
{
    public CameraManager Manager;
    public Transform CameraTransform;
    public Transform PlayerTransform;
    public Vector3 Offset;
    public float SmoothTimeFix;
    public float SmoothTimeFollow;
    public float SmoothTimeRail;
    
    public CameraSettings CurrentSettings;
    public Vector3 Velocity = Vector3.zero;
    
    public LayerMask CollisionLayers; 
    public float CollisionPadding = 0.2f;
}