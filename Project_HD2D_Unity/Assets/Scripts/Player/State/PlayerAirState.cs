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
        
            shootDirection = CalculateShootDirection(psc);
            psc.PlayerCursor.HandleRotation(shootDirection);
            psc.ShootingSystem.SetShootDirection(shootDirection);
        
            HandleCursor(psc);
        }

        public override void FixedUpdateState(PlayerStateContext psc)
        {
            
        }

        public override bool CanShoot => false;
        public override string Name => "Air";
    }
}