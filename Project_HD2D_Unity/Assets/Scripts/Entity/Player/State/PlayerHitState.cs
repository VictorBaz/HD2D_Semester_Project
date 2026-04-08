using UnityEngine;

namespace Player.State
{
    public class PlayerHitState : PlayerBaseState
    {
        private float timer;
 
        private const float STOP_DAMPING = 5f; 

        public override string Name => "Hit";

        public override void EnterState(PlayerStateContext psc)
        {
            timer = psc.PlayerData.HitDuration;
            
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
            Vector3 currentVel = psc.Rb.linearVelocity;
            
            Vector3 targetVel = new Vector3(0, currentVel.y, 0);
            psc.Rb.linearVelocity = Vector3.Lerp(currentVel, targetVel, Time.fixedDeltaTime * STOP_DAMPING);
        }

        private void Hit(PlayerStateContext psc)
        {
            psc.Rb.AddForce(psc.HitDirection * psc.PlayerData.HitForceTaken, ForceMode.Impulse);
        }

        public override bool CanMove { get; } = false;
        public override bool CanTakeDamage { get; } = false;
    }
}