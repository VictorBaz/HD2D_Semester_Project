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
    public event Action OnLockRelease;
    public event Action OnAttackMelee;
    public event Action OnJumpReleased;

    public event Action OnEnergyGive;
    public event Action OnEnergyTake;

    public event Action OnCarry;

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
        
        playerInputAction.Player.Jump.performed += ReceiveJumpPressed;
        playerInputAction.Player.Jump.canceled += ReceiveJumpReleased;
        
        playerInputAction.Player.Lock.started  += ReceiveLockToggle;
        playerInputAction.Player.Lock.canceled += ReceiveLockRelease;
        
        playerInputAction.Player.AttackMelee.performed += ReceiveAttackMelee;
        
        playerInputAction.Player.Dash.started += ReceiveDash;

        playerInputAction.Player.GiveEnergy.started += ReceiveGiveEnergy;
        playerInputAction.Player.TakeEnergy.started += ReceiveTakeEnergy;
        
        playerInputAction.Player.Carry.started += ReceiveCarry;
        
        playerInputAction.Enable();
    }

    private void StopActionsMap()
    {
        playerInputAction.Player.Move.performed -= ReceiveMove;
        playerInputAction.Player.Move.canceled -= ReceiveMove;
        
        playerInputAction.Player.Look.performed -= ReceiveShootDirection;
        playerInputAction.Player.Look.canceled -= ReceiveShootDirection;
        
        playerInputAction.Player.Jump.performed -= ReceiveJumpPressed;
        playerInputAction.Player.Jump.canceled -= ReceiveJumpReleased;
        
        playerInputAction.Player.Lock.started  -= ReceiveLockToggle;
        playerInputAction.Player.Lock.canceled -= ReceiveLockRelease;
        
        playerInputAction.Player.AttackMelee.performed -= ReceiveAttackMelee;
        
        playerInputAction.Player.Dash.started -= ReceiveDash;
        
        playerInputAction.Player.GiveEnergy.started -= ReceiveGiveEnergy;
        playerInputAction.Player.TakeEnergy.started -= ReceiveTakeEnergy;
        
        playerInputAction.Player.Carry.started -= ReceiveCarry;
        
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

    private void ReceiveJumpPressed(InputAction.CallbackContext ctx)
    {
        OnJumpPressed?.Invoke();
    }
    
    private void ReceiveJumpReleased(InputAction.CallbackContext ctx)
        => OnJumpReleased?.Invoke();

    private void ReceiveLockToggle(InputAction.CallbackContext ctx)
        => OnLockToggle?.Invoke();

    private void ReceiveLockRelease(InputAction.CallbackContext ctx)
        => OnLockRelease?.Invoke();

    private void ReceiveAttackMelee(InputAction.CallbackContext ctx)
    {
        OnAttackMelee?.Invoke();
    }

    private void ReceiveDash(InputAction.CallbackContext ctx)
    {
        OnDash?.Invoke();
    }

    private void ReceiveGiveEnergy(InputAction.CallbackContext ctx)
    {
        OnEnergyGive?.Invoke();
    }

    private void ReceiveTakeEnergy(InputAction.CallbackContext ctx)
    {
        OnEnergyTake?.Invoke();
    }

    private void ReceiveCarry(InputAction.CallbackContext ctx)
    {
        OnCarry?.Invoke();
    }
    
    #endregion
}