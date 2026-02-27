using Player.State;
using UnityEngine;

public class PlayerLocomotionState : PlayerBaseState
{
    
    private float speedMultiplier = 1f;
    
    public override void EnterState(PlayerStateContext psc)
    {
        psc.ShootingSystem.OnChargeTick += HandleChargeTick;
    }

    public override void ExitState(PlayerStateContext psc)
    {
        psc.ShootingSystem.OnChargeTick -= HandleChargeTick;
        speedMultiplier = 1f;
    }

    public override void UpdateState(PlayerStateContext psc)
    {
        
        if (!psc.Controller.IsGrounded)
        {
            psc.StateMachine.TransitionTo(new PlayerAirState());
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
        
        Vector3 shootDir = CalculateShootDirection(psc);
        psc.PlayerCursor.HandleRotation(shootDir);
        psc.ShootingSystem.SetShootDirection(shootDir);
        
        HandleCursor(psc);
        HandleAnimation(psc);
    }

    public override void FixedUpdateState(PlayerStateContext psc)
    {
        psc.Controller.UpdatePlayerControllerPhysics(targetDirection,speedMultiplier);
        psc.LockOnSystem.HandleRotationLock(psc.Rb);
    }
    
    private void HandleChargeTick(float chargeRatio)
    {
        speedMultiplier = 1f - (chargeRatio * 0.8f); 
    }
    
    public override bool CanJump   => true;
    public override bool CanAttack => true;
}
