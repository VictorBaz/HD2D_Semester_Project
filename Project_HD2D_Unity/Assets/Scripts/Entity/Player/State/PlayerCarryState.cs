using UnityEngine;

public class PlayerCarryState : PlayerBaseState
{
    public override string Name { get; protected set; } = "Carry";
    
    public override void EnterState(PlayerStateContext psc)
    {
        psc.AnimationManager.SetCarrying(true);
        psc.CurrentTargetCarry.Carry(psc.PlayerHeadTransform);
        psc.Controller.SetGravity(true);
    }

    public override void ExitState(PlayerStateContext psc)
    {
        /*if (psc.CurrentTargetCarry != null)
        {
            psc.CurrentTargetCarry.Eject();
            psc.CurrentTargetCarry = null;
        }*/
        
        psc.AnimationManager.SetCarrying(false);
    }

    public override void UpdateState(PlayerStateContext psc)
    {
        if (psc.CurrentTargetCarry != null && !psc.CurrentTargetCarry.IsCarryable())
        {
            psc.StateMachine.TransitionTo(psc.StateMachine.LocomotionState);
            return;
        }
        
        psc.LockOnSystem.CalculLockRotation();
        psc.Controller.SetLockMode(psc.LockOnSystem.IsLocked);
    
        HandleMovement(psc); 
        
        float magnitude = psc.InputManager.MoveInput.magnitude;
        
        float animMagnitude = magnitude > GameConstants.DEAD_STICK ? 1 : 0f;
        
        
        blendInput = GetBlendTreeInput(psc);
        psc.AnimationManager.HandleAnimation(
            animMagnitude,
            blendInput,
            psc.Controller.IsGrounded,
            psc.Rb.linearVelocity);
    }

    public override void FixedUpdateState(PlayerStateContext psc)
    {
        HandlePhysics(psc,0.5f);
    }
}
