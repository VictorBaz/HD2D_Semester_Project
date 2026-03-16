using Interface;
using UnityEngine;

public class PlayerStateContext
{
    public PlayerController Controller;
    public AnimationManager AnimationManager;
    public LockOnSystem LockOnSystem;
    public InputManager InputManager;
    public Rigidbody Rb;
    public Transform CameraTransform;
    public Transform PlayerTransform;
    public Transform PlayerHeadTransform;
    public PlayerManager StateMachine;
    public PlayerCursor PlayerCursor;
    public ShootingSystem ShootingSystem;
    public PlayerDataInstance PlayerData;
    public VfxManager VfxManager;
    public Vector3 ShootDirection = Vector3.zero;
    public bool JumpReleased = false;
    public bool HasDash = false;
    public ICarryable CurrentTargetCarry;
}
