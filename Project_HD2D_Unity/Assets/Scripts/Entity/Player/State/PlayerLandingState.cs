using Script.Manager;
using UnityEngine;

namespace Player.State
{
    public class PlayerLandingState : PlayerBaseState
    {
        
        
        public override void EnterState(PlayerStateContext psc)
        {
            psc.Controller.SetGravity(true);
            
            psc.Rb.linearVelocity = Vector3.zero;

            if (SoundManager.Instance)SoundManager.Instance.PlaySfx(SoundType.Jump_Land);
        }

        public override void ExitState(PlayerStateContext psc)
        {
            
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            if (psc.AnimationManager.IsLandingFinished())
            {
                psc.StateMachine.TransitionTo(psc.StateMachine.LocomotionState);
            }
            
                
        }

        public override void FixedUpdateState(PlayerStateContext psc)
        {
            
        }
        
        
        public override string Name => "Landing";

        public override bool CanJump(PlayerStateContext psc)
        {
            return !psc.LockOnSystem.IsLocked;
        }
    }
}