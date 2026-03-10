using UnityEngine;

namespace Player.State
{
    public class PlayerAirState : PlayerBaseState
    {
        private float timeInAir = 0f;
        private float maxVerticalVelocity = 12f; 
        private float gravityMultiplier = 3f;   
        private float maxGravityTime = 0.8f; 
        private float jumpCutMultiplier = 0.4f;
        

        public override string Name => "Air";
        public override bool CanShoot => false;

        public override bool CanDash => true;

        

        public override void EnterState(PlayerStateContext psc)
        {
            timeInAir          = 0f;
            psc.JumpReleased   = false;
        }

        public override void ExitState(PlayerStateContext psc)
        {
            timeInAir          = 0f;
            psc.JumpReleased   = false;
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            if (psc.Controller.IsGrounded)
            {
                psc.StateMachine.TransitionTo(psc.StateMachine.LandingState);
                return;
            }

            timeInAir += Time.deltaTime;

            psc.LockOnSystem.CalculLockRotation();
            psc.Controller.SetLockMode(psc.LockOnSystem.IsLocked);

            HandleMovement(psc);
            HandleCursor(psc);
            HandleAnimation(psc);
        }

        public override void FixedUpdateState(PlayerStateContext psc)
        {
            HandlePhysics(psc, CalculateAirControl(psc));
            ApplyFallGravity(psc);
        }

        private void ApplyFallGravity(PlayerStateContext psc)
        {
            if (psc.JumpReleased && psc.Rb.linearVelocity.y > 0)
            {
                psc.Rb.linearVelocity = new Vector3(
                    psc.Rb.linearVelocity.x,
                    psc.Rb.linearVelocity.y * jumpCutMultiplier,
                    psc.Rb.linearVelocity.z);

                psc.JumpReleased = false;
            }

            if (psc.Rb.linearVelocity.y >= 0) return;

            float gravityScale = Mathf.Lerp(1f, gravityMultiplier, timeInAir / maxGravityTime);
            
            psc.Rb.AddForce(Vector3.down * gravityScale * Physics.gravity.magnitude,
                ForceMode.Acceleration);
        }

        private float CalculateAirControl(PlayerStateContext psc)
        {
            float verticalVelocity = Mathf.Abs(psc.Rb.linearVelocity.y);
            return Mathf.Lerp(1f, 0f, verticalVelocity / maxVerticalVelocity);
        }
    }
}