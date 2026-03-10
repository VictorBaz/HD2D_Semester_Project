using System.Collections;
using Manager;
using UnityEngine;

namespace Player.State
{
    public class PlayerDashState : PlayerBaseState
    {
        private float dashSpeed    = 20f;
        private float dashDuration = 0.4f;

        private Vector3 velocityStock;

        public override string Name { get; protected set; } = "Dash";

        public override bool CanShoot  => false;
        public override bool CanMove   => false;
        public override bool CanJump   => false;
        public override bool CanAttack => false;

        public override void EnterState(PlayerStateContext psc)
        {
            velocityStock = psc.Rb.linearVelocity;
            HandleAnimation(psc);
            psc.Controller.RunRoutine(DashRoutine(psc));
        }

        public override void ExitState(PlayerStateContext psc) { }

        public override void UpdateState(PlayerStateContext psc)
        {
            HandleCursor(psc);
            HandleAnimation(psc);
        }

        public override void FixedUpdateState(PlayerStateContext psc) { }

        private IEnumerator DashRoutine(PlayerStateContext psc)
        {
            float elapsed = 0f;
            Vector3 dashDirection = psc.PlayerTransform.forward;

            EventManager.CameraShake();
            psc.VfxManager.ToggleDashTrail(true);
            
            while (elapsed < dashDuration)
            {
                psc.Rb.linearVelocity = Vector3.Lerp(
                    dashDirection * dashSpeed,
                    velocityStock * 0.7f,
                    elapsed / dashDuration);

                elapsed += Time.deltaTime;
                yield return null;
            }

            
            if (psc.Controller.IsGrounded)
            {
                psc.Rb.linearVelocity = velocityStock;
            }
            
            psc.StateMachine.TransitionTo(new PlayerLocomotionState());
            psc.VfxManager.ToggleDashTrail(false);
        }
    }
}