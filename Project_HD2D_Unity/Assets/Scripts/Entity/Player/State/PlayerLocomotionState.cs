using Player.State;
using UnityEngine;

public class PlayerLocomotionState : PlayerBaseState
{
    private float airTimeBuffer = 0f;
    
    public override void EnterState(PlayerStateContext psc)
    {
        psc.HasDash = false;
        psc.Controller.SetGravity(true);
    }

    public override void ExitState(PlayerStateContext psc)
    {
        
    }

    public override void UpdateState(PlayerStateContext psc)
    {
        
        if (!psc.Controller.IsGrounded)
        {
            airTimeBuffer += Time.deltaTime;
            
            if (airTimeBuffer > psc.PlayerData.CoyotteTime || psc.Rb.linearVelocity.y > 1f)
            {
                psc.StateMachine.TransitionTo(psc.StateMachine.AirState);
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
        
        float magnitude = psc.InputManager.MoveInput.magnitude;
        
        float animMagnitude = magnitude > psc.PlayerData.RunThreshold ? 1f :
            magnitude > GameConstants.DEAD_STICK  ? 0.5f : 0f;
        
        
        blendInput = GetBlendTreeInput(psc);
        psc.AnimationManager.HandleAnimation(
            animMagnitude,
            blendInput,
            psc.Controller.IsGrounded,
            psc.Rb.linearVelocity);
        
    }

    
    public override void FixedUpdateState(PlayerStateContext psc)
    {
        HandlePhysics(psc);
    }
    

    public override bool CanJump(PlayerStateContext psc)
    {
        return !psc.LockOnSystem.IsLocked;
    }
    public override bool CanAttack => true;

    public override bool CanDash => true;

    public override string Name => "Locomotion";
    public override bool CanCarry => true;
    public override bool CanParry => true;
}
