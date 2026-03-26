using UnityEngine;

namespace Player.State
{
    public class PlayerHitState : PlayerBaseState
    {
        private float hitDuration = 0.5f;
        private float timer;
        
        public override void EnterState(PlayerStateContext psc)
        {
            timer = hitDuration;
            
            psc.Controller.SetGravity(true);
            
            psc.AnimationManager.SetHit(true);
            Hit(psc);
        }

        public override void ExitState(PlayerStateContext psc)
        {
            psc.AnimationManager.SetHit(false);
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                psc.StateMachine.TransitionTo(psc.StateMachine.LocomotionState);
            }
        }

        public override void FixedUpdateState(PlayerStateContext psc)
        {
            
        }

        private void Hit(PlayerStateContext psc)
        {
            psc.Rb.AddForce(psc.HitDirection * 5f, ForceMode.Impulse);
        }
        

        public override bool CanMove { get; } = false;
        public override bool CanTakeDamage { get; } = false;
    }
}