using UnityEngine;

namespace Player.State
{
    public class PlayerAirState : PlayerBaseState
    {
        private float timeInAir = 0f;
        private float jumpStartTime;
        private Vector3 jumpStartPosition;
        private bool isFalling;
        
        public override string Name => "Air";
        public override bool CanDash => true;

        public override void EnterState(PlayerStateContext psc)
        {
            psc.AnimationManager.SetIsFalling(true);
            timeInAir = 0f;
            psc.JumpReleased = false;
            
            if (psc.Controller.IsJumping) 
            {
                isFalling = false;
                psc.Rb.useGravity = false; 
                jumpStartTime = Time.time;
                jumpStartPosition = psc.PlayerTransform.position;
            }
            else
            {
                StartFalling(psc);
                jumpStartTime = Time.time - 1f; 
            }
        }

        public override void ExitState(PlayerStateContext psc)
        {
            timeInAir = 0f;
            psc.JumpReleased = false;
            psc.Rb.useGravity = true; 
            psc.Controller.SetJumping(false);
            psc.AnimationManager.SetIsFalling(false);
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            float elapsed = Time.time - jumpStartTime;

            if (psc.Controller.IsGrounded && elapsed > 0.1f) 
            {
                if (elapsed < psc.PlayerData.JumpDuration * 0.6f && !isFalling)
                {
                    psc.StateMachine.TransitionTo(psc.StateMachine.LocomotionState);
                }
                else if (isFalling || psc.Rb.linearVelocity.y <= 0.1f)
                {
                    psc.StateMachine.TransitionTo(psc.StateMachine.LandingState);
                }
                
                return;
            }

            if (psc.JumpReleased && !isFalling)
            {
                StartFalling(psc);
            }

            timeInAir += Time.deltaTime;

            psc.LockOnSystem.CalculLockRotation();
            psc.Controller.SetLockMode(psc.LockOnSystem.IsLocked);

            HandleMovement(psc);
            HandleAnimation(psc);
        }

        public override void FixedUpdateState(PlayerStateContext psc)
        {
            float airControl;

            if (!isFalling)
            {
                float currentHeightReached = Mathf.Abs(psc.Rb.position.y - jumpStartPosition.y);
                float heightRatio = Mathf.Clamp01(currentHeightReached / psc.PlayerData.JumpHeight);

                airControl = Mathf.Lerp(0.2f, 1f, heightRatio);

                HandleJumpAscent(psc);
            }
            else
            {
                airControl = 1f; 
                ApplyFallGravity(psc);
            }

            HandlePhysics(psc, airControl);
        }

        private void HandleJumpAscent(PlayerStateContext psc)
        {
            float elapsed = Time.time - jumpStartTime;
            float t = elapsed / psc.PlayerData.JumpDuration;

            if (t >= 1.0f)
            {
                StartFalling(psc);
                return;
            }
    
            float heightOffset = Mathf.Sin(t * Mathf.PI * 0.5f) * psc.PlayerData.JumpHeight;
            float targetY = jumpStartPosition.y + heightOffset;

            Vector3 currentPos = psc.Rb.position;
            Vector3 nextPos = new Vector3(currentPos.x, targetY, currentPos.z);
    
            psc.Rb.MovePosition(nextPos);
        }

        private void StartFalling(PlayerStateContext psc)
        {
            isFalling = true;
            psc.Rb.useGravity = true; 
            psc.Controller.SetJumping(false);
            psc.JumpReleased = false;

            if (psc.Rb.linearVelocity.y > 0)
            {
                psc.Rb.linearVelocity = new Vector3(psc.Rb.linearVelocity.x, 0f, psc.Rb.linearVelocity.z);
            }
        }

        private void ApplyFallGravity(PlayerStateContext psc)
        {
            float gravityScale = Mathf.Lerp(1f, psc.PlayerData.GravityMultiplier, timeInAir / psc.PlayerData.MaxGravityTime);
            psc.Rb.AddForce(Vector3.down * gravityScale * Physics.gravity.magnitude, ForceMode.Acceleration);
        }
        
    }
}