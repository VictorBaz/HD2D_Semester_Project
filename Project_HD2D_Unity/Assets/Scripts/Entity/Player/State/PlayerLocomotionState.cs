using Player.State;
using UnityEngine;

public class PlayerLocomotionState : PlayerBaseState
{
    private float airTimeBuffer = 0f;
    
    public override string Name    => "Locomotion";
    public override bool CanAttack => true;
    public override bool CanDash   => true;
    public override bool CanCarry  => true;
    public override bool CanParry  => true;

    public override bool CanJump(PlayerStateContext psc) => !psc.LockOnSystem.IsLocked;

    public override void EnterState(PlayerStateContext psc)
    {
        psc.HasDash = false;
        psc.Controller.SetGravity(true);
    }

    public override void ExitState(PlayerStateContext psc) { }

    public override void UpdateState(PlayerStateContext psc)
    {
        if (!psc.Controller.IsGrounded)
        {
            airTimeBuffer += Time.deltaTime;

            if (airTimeBuffer > psc.PlayerData.CoyoteTime)
            {
                DetermineState(psc);
                return;
            }
        }
        else
        {
            airTimeBuffer = 0f;
        }

        psc.LockOnSystem.CalculLockRotation();
        psc.Controller.SetLockMode(psc.LockOnSystem.IsLocked);

        HandleMovement(psc);

        float magnitude     = psc.InputManager.MoveInput.magnitude;
        
        float animMagnitude = magnitude > psc.PlayerData.RunThreshold ? GameConstants.PLAYER_ANIM_MAGNITUDE_RUN :
            magnitude > GameConstants.DEAD_STICK ? GameConstants.PLAYER_ANIM_MAGNITUDE_WALK :
            GameConstants.PLAYER_ANIM_MAGNITUDE_IDLE;

        blendInput = GetBlendTreeInput(psc);
        psc.AnimationManager.HandleAnimation(animMagnitude, blendInput, psc.Controller.IsGrounded);
    }

    public override void FixedUpdateState(PlayerStateContext psc)
    {
        HandlePhysics(psc);
    }
}