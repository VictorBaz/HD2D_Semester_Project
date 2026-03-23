using UnityEngine;

namespace Player.State
{
    public class PlayerAirState : PlayerBaseState
    {
        private float timeInAir = 0f;
        
        public override string Name => "Air";
      

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
            if (psc.Controller.IsGrounded && psc.Rb.linearVelocity.y <= 0.1f) 
            {
                psc.StateMachine.TransitionTo(psc.StateMachine.LandingState);
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
            ApplyFallGravity(psc);
        }

        private void ApplyFallGravity(PlayerStateContext psc)
        {
            if (psc.JumpReleased && psc.Rb.linearVelocity.y > 0)
            {
                psc.Rb.linearVelocity = new Vector3(
                    psc.Rb.linearVelocity.x,
                    psc.Rb.linearVelocity.y * psc.PlayerData.JumpCutMultiplier,
                    psc.Rb.linearVelocity.z);

                psc.JumpReleased = false;
            }

            if (psc.Rb.linearVelocity.y >= 0) return;

            float gravityScale = Mathf.Lerp(1f, psc.PlayerData.GravityMultiplier, timeInAir / psc.PlayerData.MaxGravityTime);
            
            psc.Rb.AddForce(Vector3.down * gravityScale * Physics.gravity.magnitude,
                ForceMode.Acceleration);
        }

        private float CalculateAirControl(PlayerStateContext psc)
        {
            float verticalVelocity = Mathf.Abs(psc.Rb.linearVelocity.y);
            return Mathf.Lerp(1f, 0f, verticalVelocity / psc.PlayerData.MaxVerticalVelocity);
        }
    }
}