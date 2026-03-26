using UnityEngine;

namespace Player.State
{
    public class PlayerAirState : PlayerBaseState
    {
        private float timeInAir = 0f;
        private float jumpStartTime;
        private bool isFalling;
        private float requiredJumpGravity; 
        
        public override string Name => "Air";
        public override bool CanDash => true;

        public override void EnterState(PlayerStateContext psc)
        {
            psc.AnimationManager.SetFalling(true);
            timeInAir = 0f;
            jumpStartTime = Time.time;
            psc.JumpReleased = false;

            if (psc.Controller.IsJumping) 
            {
                isFalling = false;
                psc.Controller.SetGravity(false); 

                float h = psc.PlayerData.JumpHeight;
                float t = psc.PlayerData.JumpDuration;
                requiredJumpGravity = (2f * h) / (t * t);

                float jumpVelocity = requiredJumpGravity * t;

                psc.Rb.linearVelocity = new Vector3(psc.Rb.linearVelocity.x, jumpVelocity, psc.Rb.linearVelocity.z);
            }
            else
            {
                StartFalling(psc);
            }
        }

        public override void FixedUpdateState(PlayerStateContext psc)
        {
            if (!isFalling)
            {
                psc.Rb.AddForce(Vector3.down * requiredJumpGravity, ForceMode.Acceleration);
                
                if (psc.Rb.linearVelocity.y <= 0.1f) StartFalling(psc);
            }
            else
            {
                ApplyFallGravity(psc);
            }

            float airControl = Mathf.Lerp(1f, 0.4f, Mathf.Abs(psc.Rb.linearVelocity.y) / 10f);
            HandlePhysics(psc, airControl);
        }

        private void ApplyFallGravity(PlayerStateContext psc)
        {
            float timeFalling = Time.time - (jumpStartTime + psc.PlayerData.JumpDuration);
            float gravityRatio = Mathf.Clamp01(timeFalling / psc.PlayerData.MaxGravityTime);
            
            float currentMultiplier = Mathf.Lerp(1f, psc.PlayerData.GravityMultiplier, gravityRatio);
            psc.Rb.AddForce(Vector3.down * currentMultiplier * Physics.gravity.magnitude, ForceMode.Acceleration);
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            if (psc.JumpReleased && !isFalling && psc.Rb.linearVelocity.y > 0)
            {
                psc.Rb.linearVelocity = new Vector3(psc.Rb.linearVelocity.x, psc.Rb.linearVelocity.y * 0.5f, psc.Rb.linearVelocity.z);
                StartFalling(psc);
            }

            if (psc.Controller.IsGrounded && (Time.time - jumpStartTime) > 0.1f) 
            {
                psc.StateMachine.TransitionTo(psc.StateMachine.LandingState);
                return;
            }

            timeInAir += Time.deltaTime;
            HandleMovement(psc);
            HandleAnimation(psc);
        }

        private void StartFalling(PlayerStateContext psc)
        {
            isFalling = true;
            psc.Controller.SetGravity(true); 
            psc.Controller.SetJumping(false);
        }

        public override void ExitState(PlayerStateContext psc)
        {
            psc.Controller.SetGravity(true);
            psc.AnimationManager.SetFalling(false);
        }
    }
}