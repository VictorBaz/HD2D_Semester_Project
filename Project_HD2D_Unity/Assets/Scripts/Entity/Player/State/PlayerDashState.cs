using System.Collections;
using Manager;
using UnityEngine;

namespace Player.State
{
    public class PlayerDashState : PlayerBaseState
    {
        private float velocityStock;
        
        public override string Name { get; protected set; } = "Dash";

        public override bool CanMove => false;
        public override bool CanAttack => false;
        
        

        public override void EnterState(PlayerStateContext psc)
        {
            if (!psc.Controller.IsGrounded)
            {
                psc.HasDash = true;
            }
            
            psc.Controller.SetGravity(false);
            
            psc.AnimationManager.SetDashing(true);
            
            velocityStock = psc.Rb.linearVelocity.magnitude;
            
            HandleAnimation(psc);
            
            psc.Controller.RunRoutine(DashRoutine(psc));
        }

        public override void ExitState(PlayerStateContext psc)
        {
            psc.AnimationManager.SetDashing(false);
            
            psc.Controller.SetGravity(true);
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
                    Vector3.zero,
                    elapsed / psc.PlayerData.DashDuration);
                
                psc.Rb.linearVelocity = new Vector3(psc.Rb.linearVelocity.x, 0, psc.Rb.linearVelocity.z);

                elapsed += Time.deltaTime;
                yield return null;
            }
            
            elapsed = 0f;

            Vector3 dashVelocityExit = psc.TargetDirection * velocityStock;
            
            while (elapsed < 0.01f)
            {
                psc.Rb.linearVelocity = Vector3.Lerp(Vector3.zero, dashVelocityExit, elapsed / 0.1f);
                
                elapsed += Time.deltaTime;
                
                yield return null;
            }
            
            psc.Rb.linearVelocity = dashVelocityExit;
            
            DetermineState(psc);
            
            psc.VfxManager.ToggleDashTrail(false);
        }
    }
}