using System.Collections;
using Manager;
using UnityEngine;

namespace Player.State
{
    public class PlayerDashState : PlayerBaseState
    {
        private Vector3 velocityStock;
        
        public override string Name { get; protected set; } = "Dash";

        public override bool CanShoot => false;
        public override bool CanMove => false;
        public override bool CanAttack => false;
        
        

        public override void EnterState(PlayerStateContext psc)
        {
            if (!psc.Controller.IsGrounded)
            {
                psc.HasDash = true;
            }
            
            psc.AnimationManager.SetDash(true);
            velocityStock = psc.Rb.linearVelocity;
            HandleAnimation(psc);
            psc.Controller.RunRoutine(DashRoutine(psc));
        }

        public override void ExitState(PlayerStateContext psc)
        {
            psc.AnimationManager.SetDash(false);
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            HandleAnimation(psc);
        }

        public override void FixedUpdateState(PlayerStateContext psc) { }

        private IEnumerator DashRoutine(PlayerStateContext psc)
        {
            float elapsed = 0f;
            Vector3 dashDirection = psc.PlayerTransform.forward;

            EventManager.CameraShake();
            psc.VfxManager.ToggleDashTrail(true);
            
            while (elapsed < psc.PlayerData.DashDuration)
            {
                psc.Rb.linearVelocity = Vector3.Lerp(
                    dashDirection * psc.PlayerData.DashSpeed,
                    velocityStock * 0.7f,
                    elapsed / psc.PlayerData.DashDuration);

                elapsed += Time.deltaTime;
                yield return null;
            }

            bool isGrounded = psc.Controller.IsGrounded;
            
            if (isGrounded)
            {
                psc.Rb.linearVelocity = velocityStock;
            }
            
            psc.StateMachine.TransitionTo(isGrounded ? psc.StateMachine.LocomotionState 
                : psc.StateMachine.AirState);
            
            psc.VfxManager.ToggleDashTrail(false);
        }
    }
}