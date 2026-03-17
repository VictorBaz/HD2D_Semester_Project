using Player.State;
using UnityEngine;

public class PlayerLocomotionState : PlayerBaseState
{
    private float speedMultiplier = 1f;
    private float airTimeBuffer = 0f;
    private const float MaxAirTimeBeforeFall = 0.2f;
    
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
            airTimeBuffer += Time.deltaTime;
            
            if (airTimeBuffer > MaxAirTimeBeforeFall || psc.Rb.linearVelocity.y > 1f)
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
            magnitude > 0.05f  ? 0.5f : 0f;
        
        
        blendInput = GetBlendTreeInput(psc);
        psc.AnimationManager.HandleAnimation(
            animMagnitude,
            blendInput,
            psc.Controller.IsGrounded,
            psc.Rb.linearVelocity);
        
    }

    
    public override void FixedUpdateState(PlayerStateContext psc)
    {
        HandlePhysics(psc,speedMultiplier);
        psc.LockOnSystem.HandleRotationLock(psc.Rb);
    }
    

    public override bool CanJump(PlayerStateContext psc)
    {
        return !psc.LockOnSystem.IsLocked;
    }
    public override bool CanAttack => true;

    public override bool CanDash => true;

    public override string Name => "Locomotion";
    public override bool CanCarry => true;
}
