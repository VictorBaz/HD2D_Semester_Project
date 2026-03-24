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
            timeInAir = 0f;
            psc.JumpReleased = false;
            
            // FIX : On ne lance la montée QUE si on vient de presser Jump
            // Si on vient d'un Dash ou d'une chute, IsJumping sera false.
            if (psc.Controller.IsJumping) 
            {
                isFalling = false;
                psc.Rb.useGravity = false; 
                jumpStartTime = Time.time;
                jumpStartPosition = psc.PlayerTransform.position;
            }
            else
            {
                // On tombe directement (cas du Dash fini en l'air ou chute de rebord)
                StartFalling(psc);
                jumpStartTime = Time.time - 1f; // On simule un saut vieux pour le Landing
            }
        }

        public override void ExitState(PlayerStateContext psc)
        {
            timeInAir = 0f;
            psc.JumpReleased = false;
            psc.Rb.useGravity = true; 
            psc.Controller.SetJumping(false);
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
            HandlePhysics(psc, CalculateAirControl(psc));

            if (!isFalling)
            {
                HandleJumpAscent(psc);
            }
            else
            {
                ApplyFallGravity(psc);
            }
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

        private float CalculateAirControl(PlayerStateContext psc)
        {
            float verticalVelocity = Mathf.Abs(psc.Rb.linearVelocity.y);
            return Mathf.Lerp(1f, 0.2f, verticalVelocity / psc.PlayerData.MaxVerticalVelocity);
        }
    }
}