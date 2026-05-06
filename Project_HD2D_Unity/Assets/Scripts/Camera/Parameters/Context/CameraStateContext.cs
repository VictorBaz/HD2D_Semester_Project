using UnityEngine;

public class CameraStateContext
{
    public CameraManager Manager;
    public Transform CameraTransform;
    public Transform PlayerTransform;
    public Vector3 Offset;
    public Vector3 Velocity = Vector3.zero;
    
    public CameraSettings CurrentSettings;
    public float TransitionSpeed; 
    
    public LayerMask CollisionLayers; 
    public float CollisionPadding = 0.2f;
}