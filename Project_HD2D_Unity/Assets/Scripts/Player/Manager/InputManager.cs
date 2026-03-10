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

    public event Action OnShootStart;
    public event Action OnShootStop;

    public event Action OnDash;
    
    private PlayerInputAction playerInputAction;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
    }

    private void OnEnable()
    {
        StopActionsMap();
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

        playerInputAction.Player.Shoot.started += ReceiveShootStart;
        playerInputAction.Player.Shoot.canceled += ReceiveShootStop;
        
        
        playerInputAction.Player.Dash.started += ReceiveDash;
        
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
        
        playerInputAction.Player.Shoot.started -= ReceiveShootStart;
        playerInputAction.Player.Shoot.canceled -= ReceiveShootStop;
        
        playerInputAction.Player.Dash.started -= ReceiveDash;
        
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

    private void ReceiveShootStart(InputAction.CallbackContext ctx)
    {
        OnShootStart?.Invoke();
    }
    
    private void ReceiveShootStop(InputAction.CallbackContext ctx)
    {
        OnShootStop?.Invoke();
    }

    private void ReceiveDash(InputAction.CallbackContext ctx)
    {
        OnDash?.Invoke();
    }
    
    #endregion
}