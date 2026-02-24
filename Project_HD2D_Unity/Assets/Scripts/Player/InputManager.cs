using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Variables

    public Vector2 MoveInput { get; private set; }
    public Vector2 ShootInput { get; private set; }

    public event Action OnJumpPressed;
    public event Action OnLockToggle;
    public event Action OnAttackMelee;

    public event Action<InputAction.CallbackContext> OnShoot;
    
    private PlayerInputAction playerInputAction;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
    }

    private void OnEnable()
    {
        SetupActionsMap();
    }

    private void OnDisable()
    {
        StopActionsMap();
    }

    #endregion

    #region Setup

    private void SetupActionsMap()
    {
        playerInputAction.Player.Move.performed += ReceiveMove;
        playerInputAction.Player.Move.canceled += ReceiveMove;

        playerInputAction.Player.Look.performed += ReceiveShootDirection;
        playerInputAction.Player.Look.canceled += ReceiveShootDirection;
        
        playerInputAction.Player.Jump.performed += ReceiveJump;
        
        playerInputAction.Player.Lock.performed += ReceiveLockToggle;
        
        playerInputAction.Player.AttackMelee.performed += ReceiveAttackMelee;

        playerInputAction.Player.Shoot.started += ReceiveShoot;
        playerInputAction.Player.Shoot.canceled += ReceiveShoot;
        
        playerInputAction.Enable();
    }

    private void StopActionsMap()
    {
        playerInputAction.Player.Move.performed -= ReceiveMove;
        playerInputAction.Player.Move.canceled -= ReceiveMove;
        
        playerInputAction.Player.Look.performed -= ReceiveShootDirection;
        playerInputAction.Player.Look.canceled -= ReceiveShootDirection;
        
        playerInputAction.Player.Jump.performed -= ReceiveJump;
        
        playerInputAction.Player.Lock.performed -= ReceiveLockToggle;
        
        playerInputAction.Player.AttackMelee.performed -= ReceiveAttackMelee;
        
        playerInputAction.Player.Shoot.started -= ReceiveShoot;
        playerInputAction.Player.Shoot.canceled -= ReceiveShoot;
        
        playerInputAction.Disable();
    }

    #endregion

    #region Inputs

    private void ReceiveMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void ReceiveShootDirection(InputAction.CallbackContext ctx)
    {
        ShootInput = ctx.ReadValue<Vector2>();
    }

    private void ReceiveJump(InputAction.CallbackContext ctx)
    {
        OnJumpPressed?.Invoke();
    }

    private void ReceiveLockToggle(InputAction.CallbackContext ctx)
    {
        OnLockToggle?.Invoke();
    }

    private void ReceiveAttackMelee(InputAction.CallbackContext ctx)
    {
        OnAttackMelee?.Invoke();
    }

    private void ReceiveShoot(InputAction.CallbackContext ctx)
    {
        OnShoot?.Invoke(ctx);
    }
    
    #endregion
}