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
        
            Vector3 shootDir = CalculateShootDirection(psc);
            psc.PlayerCursor.HandleRotation(shootDir);
            psc.ShootingSystem.SetShootDirection(shootDir);
        
            HandleCursor(psc);
        }

        public override void FixedUpdateState(PlayerStateContext psc)
        {
            
        }

        public override bool CanShoot => false;
    }
}