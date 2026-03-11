namespace Player.State
{
    public class PlayerLandingState : PlayerBaseState
    {
        
        
        public override void EnterState(PlayerStateContext psc)
        {
         
        }

        public override void ExitState(PlayerStateContext psc)
        {
            
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            if (psc.AnimationManager.IsLandingFinished())
                psc.StateMachine.TransitionTo(psc.StateMachine.LocomotionState);
        }

        public override void FixedUpdateState(PlayerStateContext psc)
        {
            
        }
        
        
        public override string Name => "Landing";

        public override bool CanJump { get; } = true;
    }
}