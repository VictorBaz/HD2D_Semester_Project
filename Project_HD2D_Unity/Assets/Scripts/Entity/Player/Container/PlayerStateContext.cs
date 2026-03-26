using Interface;
using UnityEngine;

public class PlayerStateContext
{
    public PlayerAnimationManager AnimationManager;
    public LockOnSystem LockOnSystem;
    public InputManager InputManager;
    public VfxManager VfxManager;
    
    public PlayerController Controller;
    
    public Rigidbody Rb;
    
    public Transform CameraTransform;
    public Transform PlayerTransform;
    public Transform PlayerHeadTransform;
    
    public PlayerDataInstance PlayerData;
    
    public Vector3 ShootDirection = Vector3.zero;
    public Vector3 HitDirection = Vector3.zero;
    
    public bool JumpReleased = false;
    public bool HasDash = false;
    
    public ICarryable CurrentTargetCarry;
    
    public PlayerManager StateMachine;

    public Vector3 TargetDirection;
}
