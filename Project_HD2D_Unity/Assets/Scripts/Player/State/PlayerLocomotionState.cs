using Player.State;
using UnityEngine;

public class PlayerLocomotionState : PlayerBaseState
{
    private float speedMultiplier = 1f;
    
    public override void EnterState(PlayerStateContext psc)
    {
        psc.HasDash = false;
    }

    public override void ExitState(PlayerStateContext psc)
    {
        speedMultiplier = 1f;
    }

    public override void UpdateState(PlayerStateContext psc)
    {
        
        if (!psc.Controller.IsGrounded)
        {
            psc.StateMachine.TransitionTo(psc.StateMachine.AirState);
            return;
        }
    
        psc.LockOnSystem.CalculLockRotation();
        psc.Controller.SetLockMode(psc.LockOnSystem.IsLocked);
    
        HandleMovement(psc); 
        
        blendInput = GetBlendTreeInput(psc);
        psc.AnimationManager.HandleAnimation(
            psc.Rb.linearVelocity.magnitude,
            blendInput,
            psc.Controller.IsGrounded);
        
        HandleAnimation(psc);
    }

    public override void FixedUpdateState(PlayerStateContext psc)
    {
        HandlePhysics(psc,speedMultiplier);
        psc.LockOnSystem.HandleRotationLock(psc.Rb);
    }
    
    private void HandleChargeTick(float chargeRatio)
    {
        speedMultiplier = 1f - (chargeRatio * 0.8f); 
    }
    
    public override bool CanJump   => true;
    public override bool CanAttack => true;

    public override bool CanDash => true;

    public override string Name => "Locomotion";
}
