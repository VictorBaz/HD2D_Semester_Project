using UnityEngine;

namespace Player.State
{
    public class PlayerAirState : PlayerBaseState
    {
        private float timeInAir = 0f;
        private bool isFalling;
        
        public override string Name => "Air";
        public override bool CanDash => true;

        public override void EnterState(PlayerStateContext psc)
        {
            timeInAir = 0f;
            psc.JumpReleased = false;

            if (psc.Controller.IsJumping) 
            {
                isFalling = false;
                psc.Controller.SetGravity(true); 

                float gravityForce = Physics.gravity.magnitude * psc.PlayerData.GravityMultiplier;
                float jumpVelocity = Mathf.Sqrt(2f * gravityForce * psc.PlayerData.JumpHeight);

                psc.Rb.linearVelocity = new Vector3(psc.Rb.linearVelocity.x, jumpVelocity, psc.Rb.linearVelocity.z);
            }
            else
            {
                StartFalling(psc);
            }
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            if (!isFalling && psc.Rb.linearVelocity.y <= 0.1f)
            {
                isFalling = true;
            }

            if (psc.JumpReleased && !isFalling)
            {
                psc.Rb.linearVelocity = new Vector3(psc.Rb.linearVelocity.x, psc.Rb.linearVelocity.y * 0.5f, psc.Rb.linearVelocity.z);
                isFalling = true;
            }

            if (psc.Controller.IsGrounded && timeInAir > 0.1f) 
            {
                psc.StateMachine.TransitionTo(psc.StateMachine.LandingState);
                return;
            }

            timeInAir += Time.deltaTime;
            HandleMovement(psc);
            HandleAnimation(psc);
        }

        public override void FixedUpdateState(PlayerStateContext psc)
        {
            ApplyFallGravity(psc);
            
            float airControl = Mathf.Lerp(1f, 0.4f, Mathf.Abs(psc.Rb.linearVelocity.y) / 10f);
            HandlePhysics(psc, airControl);
        }

        private void StartFalling(PlayerStateContext psc)
        {
            isFalling = true;
            psc.Controller.SetGravity(true);
            psc.Controller.SetJumping(false);
        }

        private void ApplyFallGravity(PlayerStateContext psc)
        {
            float multiplier = isFalling ? psc.PlayerData.GravityMultiplier : 1f;
            psc.Rb.AddForce(Vector3.down * multiplier * Physics.gravity.magnitude, ForceMode.Acceleration);
        }
        
        public override void ExitState(PlayerStateContext psc)
        {
            psc.Controller.SetGravity(true);
            psc.AnimationManager.SetFalling(false);
        }
    }
}