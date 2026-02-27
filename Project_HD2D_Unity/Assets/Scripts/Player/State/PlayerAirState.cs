using UnityEngine;

namespace Player.State
{
    public class PlayerAirState : PlayerBaseState
    {
        public override void EnterState(PlayerStateContext psc)
        {
            
        }

        public override void ExitState(PlayerStateContext psc)
        {
            
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            if (psc.Controller.IsGrounded)
            {
                psc.StateMachine.TransitionTo(new PlayerLandingState());
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
            
        }
        
        
    }
}