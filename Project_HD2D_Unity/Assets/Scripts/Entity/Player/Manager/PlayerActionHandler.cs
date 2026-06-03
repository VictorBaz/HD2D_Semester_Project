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
    [SerializeField] private VfxManagerPlayer vfxManagerPlayer;

    private PlayerManager pm;

    private float dashCooldownTimer  = 0f;
    private float jumpCooldownTimer  = 0f;
    private float parryCooldownTimer = 0f;
    
    [SerializeField] private bool unlockParry = false;

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
        if (pm.Context.LockOnSystem.IsLocked) return;
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
            Vector3 forceMondiale = transform.TransformDirection(Data.EjectionForce);
            pm.Context.CurrentTargetCarry.Eject(forceMondiale);
            pm.Context.CurrentTargetCarry = null;
            pm.TransitionTo(pm.LocomotionState);
            return;
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
        if (!unlockParry) return;
        if (parryCooldownTimer > 0f) return;
        if (lockOnSystem.IsLocked) return;
        if (pm.CurrentPlayerState is PlayerParryState) return;
        if (!pm.CurrentPlayerState.CanParry) return;

        parryCooldownTimer = Data.ParryCooldown;
        pm.TransitionTo(pm.ParryState);
    }

    public void UnlockParry()
    {
        unlockParry = true;
    }
    
    #endregion

    #region Lock On

    private void OnLockToggle()
    {
        lockOnSystem.TryLock();
        UiEvents.TriggerLockStateChanged(lockOnSystem.IsLocked);
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
        if (!TryGetFlawTarget(out Flaw flaw)) return;
        
        if (Data.IsEnergyEmpty() || flaw.IsAtMaximumEnergy() || !flaw.IsLockable()) return;

        flaw.AddEnergy();
        Data.RemoveEnergy();
        UiEvents.TriggerEnergyChanged(Data.Energy, Data.MaxEnergy);
        SoundManager.Instance?.PlaySfx(SoundType.Fissure_Energy_In);
        vfxManagerPlayer.EffectAddEnergy();
        GamepadVibrationHelper.Vibrate(0.25f,1f,0.25f);
        
        flaw.energyDisplay.Show(flaw.root.CurrentEnergy);
    }

    private void TryTakeEnergy()
    {
        if (!TryGetFlawTarget(out Flaw flaw)) return;
        
        if (Data.Energy >= Data.MaxEnergy || !flaw.IsContainingEnergy()) return;

        flaw.RemoveEnergy();
        Data.AddEnergy();
        UiEvents.TriggerEnergyChanged(Data.Energy, Data.MaxEnergy);
        SoundManager.Instance?.PlaySfx(SoundType.Fissure_Energy_Out);
        vfxManagerPlayer.EffectRemoveEnergy();
        GamepadVibrationHelper.Vibrate(0.15f,0.5f,0.25f);
        
        flaw.energyDisplay.Show(flaw.root.CurrentEnergy);
    }

    private bool TryGetFlawTarget(out Flaw flaw)
    {
        flaw = null;
        if (!TryGetEnergyTarget(out IEnergyLockable target)) return false;
        if (target is not Flaw f || f.IsBlocked()) return false;
        flaw = f;
        return true;
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

}