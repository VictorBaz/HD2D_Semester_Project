using System.Collections;
using Script.Manager;
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
            psc.VfxManagerPlayer.ToggleDashTrail(false);
            psc.Controller.SetGravity(true);
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            HandleAnimation(psc);
            
            if (psc.Controller.IsFacingWall())
            {
                float yStock = psc.Rb.linearVelocity.y;
                psc.Rb.linearVelocity = Vector3.zero + new Vector3(0, yStock, 0);
                DetermineState(psc);
            }
        }

        public override void FixedUpdateState(PlayerStateContext psc) { }

        private IEnumerator DashRoutine(PlayerStateContext psc)
        {
            float elapsed = 0f;
            Vector3 dashDirection = psc.PlayerTransform.forward;

            CameraEvents.CameraShake();
            SoundManager.Instance?.PlaySfx(SoundType.Dash);
            psc.VfxManagerPlayer.ToggleDashTrail(true);

            while (elapsed < psc.PlayerData.DashDuration)
            {/*
                float t = elapsed / psc.PlayerData.DashDuration;*/

                psc.Rb.linearVelocity = dashDirection * psc.PlayerData.DashSpeed;/*Vector3.Lerp(
                    dashDirection * psc.PlayerData.DashSpeed,
                    dashDirection * (psc.PlayerData.DashSpeed * 0.08f),
                    t);*/
                
                psc.Rb.linearVelocity = new Vector3(
                    psc.Rb.linearVelocity.x,
                    0,
                    psc.Rb.linearVelocity.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            float exitDuration = 0.18f;

            Vector3 velocityAtEndOfDash = psc.Rb.linearVelocity;

            Vector3 dashVelocityExit = psc.TargetDirection.magnitude > 0.1f
                ? psc.TargetDirection * velocityStock
                : psc.PlayerTransform.forward * velocityStock;

            while (elapsed < exitDuration)
            {
                float t = elapsed / exitDuration;
                float smoothT = t * t * (3f - 2f * t);
                
                psc.Rb.linearVelocity = Vector3.Lerp(velocityAtEndOfDash, dashVelocityExit, smoothT);
                
                elapsed += Time.deltaTime;
                yield return null;
            }

            psc.Rb.linearVelocity = dashVelocityExit;
            
            DetermineState(psc);
            psc.VfxManagerPlayer.ToggleDashTrail(false);
        }
    }
}