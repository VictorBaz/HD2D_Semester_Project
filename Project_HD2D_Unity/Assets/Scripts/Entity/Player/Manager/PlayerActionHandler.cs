using Interface;
using Player.State;
using Script.Manager;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerActionHandler : MonoBehaviour
{
    #region Variables

    [SerializeField] private InputManager    inputManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAnimationManager animationManager;
    [SerializeField] private LockOnSystem    lockOnSystem;

    private PlayerManager pm;

    private float dashCooldownTimer  = 0f;
    private float jumpCooldownTimer  = 0f;
    private float parryCooldownTimer = 0f;

    private PlayerDataInstance Data => pm.Context.PlayerData;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        pm = GetComponent<PlayerManager>();
    }

    private void OnEnable()
    {
        inputManager.OnLockToggle   += OnLockToggle;
        inputManager.OnLockRelease  += OnLockRelease;

        inputManager.OnJumpPressed  += TryJump;
        inputManager.OnJumpReleased += TryJumpReleased;
        playerController.OnJump     += animationManager.TriggerJump;

        inputManager.OnAttackMelee  += TryAttack;
        inputManager.OnDash         += TryDash;
        inputManager.OnCarry        += TryCarry;
        inputManager.OnParry        += TryParry;

        inputManager.OnEnergyGive   += TryGiveEnergy;
        inputManager.OnEnergyTake   += TryTakeEnergy;

        if (GameManager.Instance != null)
            inputManager.OnPausePressed += GameManager.Instance.TogglePause;

        PlayerEvents.OnRequestCurrentLockTarget = GetCurrentTargetLock;
    }

    private void OnDisable()
    {
        inputManager.OnLockToggle   -= OnLockToggle;
        inputManager.OnLockRelease  -= OnLockRelease;

        inputManager.OnJumpPressed  -= TryJump;
        inputManager.OnJumpReleased -= TryJumpReleased;
        playerController.OnJump     -= animationManager.TriggerJump;

        inputManager.OnAttackMelee  -= TryAttack;
        inputManager.OnDash         -= TryDash;
        inputManager.OnCarry        -= TryCarry;
        inputManager.OnParry        -= TryParry;

        inputManager.OnEnergyGive   -= TryGiveEnergy;
        inputManager.OnEnergyTake   -= TryTakeEnergy;

        if (GameManager.Instance != null)
            inputManager.OnPausePressed -= GameManager.Instance.TogglePause;
    }

    private void Update()
    {
        TickTimers();
        playerController.SetJumping(jumpCooldownTimer > 0 || pm.CurrentPlayerState is PlayerBumpState);
    }

    #endregion

    #region Timers

    private void TickTimers()
    {
        if (dashCooldownTimer  > 0f) dashCooldownTimer  -= Time.deltaTime;
        if (jumpCooldownTimer  > 0f) jumpCooldownTimer  -= Time.deltaTime;
        if (parryCooldownTimer > 0f) parryCooldownTimer -= Time.deltaTime;
    }

    #endregion

    #region Jump

    private void TryJump()
    {
        if (!pm.CurrentPlayerState.CanJump(pm.Context)) return;
        if (jumpCooldownTimer > 0f) return;

        jumpCooldownTimer = Data.JumpCooldown;
        playerController.Jump();
        pm.TransitionTo(pm.JumpState);
    }

    private void TryJumpReleased()
    {
        if (pm.CurrentPlayerState is PlayerJumpState)
            pm.Context.JumpReleased = true;
    }

    #endregion

    #region Attack

    private void TryAttack()
    {
        if (lockOnSystem.IsLocked) return;

        if (pm.CurrentPlayerState is PlayerAttackState meleeState)
        {
            meleeState.BufferAttack();
            return;
        }

        if (!pm.CurrentPlayerState.CanAttack) return;

        pm.TransitionTo(pm.AttackState);
    }

    #endregion

    #region Dash

    private void TryDash()
    {
        if (!pm.CurrentPlayerState.CanDash) return;
        if (dashCooldownTimer > 0f) return;
        if (pm.Context.HasDash) return;

        if (pm.CurrentPlayerState is PlayerInAirBase)
            pm.Context.HasDash = true;

        dashCooldownTimer = Data.DashCooldown;
        pm.TransitionTo(pm.DashState);
    }

    #endregion

    #region Carry

    private void TryCarry()
    {
        if (pm.Context.CurrentTargetCarry != null)
        {
            pm.Context.CurrentTargetCarry.Eject();
            pm.Context.CurrentTargetCarry = null;
            pm.TransitionTo(pm.LocomotionState);
        }

        if (!pm.CurrentPlayerState.CanCarry) return;

        var targets = DetectionHelper.FindVisibleTargets<ICarryable>(
            transform, Data.CarryRange, Data.CarryAngle, Data.CarryLayer);

        targets.RemoveAll(t => !t.IsCarryable());

        pm.Context.CurrentTargetCarry = DetectionHelper.GetBestTarget(transform, targets);

        if (pm.Context.CurrentTargetCarry != null)
            pm.TransitionTo(pm.CarryState);
    }

    #endregion

    #region Parry

    private void TryParry()
    {
        if (parryCooldownTimer > 0f) return;
        if (lockOnSystem.IsLocked) return;
        if (pm.CurrentPlayerState is PlayerParryState) return;
        if (!pm.CurrentPlayerState.CanParry) return;

        parryCooldownTimer = Data.ParryCooldown;
        pm.TransitionTo(pm.ParryState);
    }

    #endregion

    #region Lock On

    private void OnLockToggle()
    {
        lockOnSystem.TryLock();
        UiEvents.TriggerLockStateChanged(lockOnSystem.IsLocked);
        TryGiveSap();
    }

    private void OnLockRelease()
    {
        lockOnSystem.Unlock();
        UiEvents.TriggerLockStateChanged(false);
    }

    private Transform GetCurrentTargetLock()
        => lockOnSystem.CurrentTarget?.GetLockTransform();

    #endregion

    #region Energy

    private void TryGiveEnergy()
    {
        if (!TryGetEnergyTarget(out IEnergyLockable target)) return;
        if (Data.IsEnergyEmpty()) return;
        if (target.IsAtMaximumEnergy()) return;

        target.AddEnergy();
        Data.RemoveEnergy();
        UiEvents.TriggerEnergyChanged(Data.Energy, Data.MaxEnergy);
        SoundManager.Instance?.PlaySfx(SoundType.Fissure_Energy_In);
    }

    private void TryTakeEnergy()
    {
        if (pm.Context.PlayerData.Energy >= pm.Context.PlayerData.MaxEnergy) return;
        if (!TryGetEnergyTarget(out IEnergyLockable target)) return;
        if (!target.IsContainingEnergy()) return;

        target.RemoveEnergy();
        Data.AddEnergy();
        UiEvents.TriggerEnergyChanged(Data.Energy, Data.MaxEnergy);
        SoundManager.Instance?.PlaySfx(SoundType.Fissure_Energy_Out);
    }

    private bool TryGetEnergyTarget(out IEnergyLockable target)
    {
        target = null;
        if (!lockOnSystem.IsLocked) return false;
        if (lockOnSystem.CurrentTarget is not IEnergyLockable e) return false;
        target = e;
        return true;
    }

    #endregion

    #region Sap

    private void TryGiveSap()
    {/*
        var targets = DetectionHelper.FindVisibleTargets<ISapLockable>(
            transform, Data.LockRange, Data.LockAngle, Data.SapLayerMask);

        targets.RemoveAll(t => !t.IsLockable());

        ISapLockable sap = DetectionHelper.GetBestTarget(transform, targets);

        if (sap == null) return;

        sap.GiveSap();
        Data.AddSap();
        UiEvents.TriggerSapChanged(Data.Sap);*/
    }

    #endregion
}