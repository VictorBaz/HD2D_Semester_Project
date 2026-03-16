using System.Collections;

namespace Player.State
{
    public class PlayerHitState : PlayerBaseState
    {
        
        public override void EnterState(PlayerStateContext psc)
        {
            
        }

        public override void ExitState(PlayerStateContext psc)
        {
            
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            
        }

        public override void FixedUpdateState(PlayerStateContext psc)
        {
            
        }

        private IEnumerator HurtIe(PlayerStateContext psc)
        {
            yield return null;
            psc.StateMachine.TransitionTo(psc.StateMachine.LocomotionState);
        }

        public override bool CanMove { get; } = false;
        public override bool CanTakeDamage { get; } = false;
    }
}