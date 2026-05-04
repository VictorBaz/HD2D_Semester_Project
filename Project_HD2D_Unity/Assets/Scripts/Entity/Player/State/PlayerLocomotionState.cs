using Player.State;
using Script.Manager;
using UnityEngine;

public class PlayerLocomotionState : PlayerBaseState
{
    private float airTimeBuffer = 0f;

    public override string Name    => "Locomotion";
    public override bool CanAttack => true;
    public override bool CanDash   => true;
    public override bool CanCarry  => true;
    public override bool CanParry  => true;
    private float nextFootstepTime = 0;
    private bool isRightFeetSound;

    public override bool CanJump(PlayerStateContext psc) => !psc.LockOnSystem.IsLocked;

    public override void EnterState(PlayerStateContext psc)
    {
        psc.HasDash = false;
        psc.Controller.SetGravity(true);
        psc.VfxManagerPlayer.PlayDust(true);
    }

    public override void ExitState(PlayerStateContext psc)
    {
        psc.VfxManagerPlayer.PlayDust(false);
    }

    public override void UpdateState(PlayerStateContext psc)
    {
        if (TryLeaveGround(psc)) return;

        psc.LockOnSystem.CalculLockRotation();
        psc.Controller.SetLockMode(psc.LockOnSystem.IsLocked);

        HandleMovement(psc);

        float magnitude = psc.InputManager.MoveInput.magnitude;
        
        float animMagnitude = magnitude > psc.PlayerData.RunThreshold ? GameConstants.ANIM_MAGNITUDE_RUN :
            magnitude > GameConstants.DEAD_STICK ? GameConstants.ANIM_MAGNITUDE_WALK :
            GameConstants.ANIM_MAGNITUDE_IDLE;

        blendInput = GetBlendTreeInput(psc);
        psc.AnimationManager.HandleAnimation(animMagnitude, blendInput, psc.Controller.IsGrounded);
        PlaySoundStep(psc);
    }

    public override void FixedUpdateState(PlayerStateContext psc)
    {
        HandlePhysics(psc);
    }

    private bool TryLeaveGround(PlayerStateContext psc)
    {
        if (!psc.Controller.IsGrounded)
        {
            airTimeBuffer += Time.deltaTime;

            if (airTimeBuffer > psc.PlayerData.CoyoteTime)
            {
                DetermineState(psc);
                return true;
            }
        }
        else
        {
            airTimeBuffer = 0f;
        }

        return false;
    }

    private void HandleAnimationLocomotion(PlayerStateContext psc)
    {
        float magnitude     = psc.InputManager.MoveInput.magnitude;
        float animMagnitude = GetAnimMagnitude(psc, magnitude);

        blendInput = GetBlendTreeInput(psc);
        psc.AnimationManager.HandleAnimation(animMagnitude, blendInput, psc.Controller.IsGrounded);
    }

    private float GetAnimMagnitude(PlayerStateContext psc, float magnitude)
    {
        if (magnitude > psc.PlayerData.RunThreshold)  return GameConstants.ANIM_MAGNITUDE_RUN;
        if (magnitude > GameConstants.DEAD_STICK)     return GameConstants.ANIM_MAGNITUDE_WALK;
        return GameConstants.ANIM_MAGNITUDE_IDLE;
    }

    
    private void PlaySoundStep(PlayerStateContext psc)
    {
        if (psc.InputManager.MoveInput.magnitude < GameConstants.DEAD_STICK) return;

        if (!(Time.time >= nextFootstepTime)) return;
        
        isRightFeetSound = !isRightFeetSound;
        
        SoundType step = isRightFeetSound ? SoundType.Footstep_Dirt_1 : SoundType.Footstep_Dirt_2;
        
        SoundManager.Instance?.PlaySfx(step);
        
        float interval = psc.InputManager.MoveInput.magnitude > psc.PlayerData.RunThreshold ? 0.3f : 0.5f;
        nextFootstepTime = Time.time + interval;
    }
}