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
        
        private bool hitAWall = false;

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

            psc.VfxManagerPlayer.TriggerDashVfx();

            hitAWall = false;
        }

        public override void ExitState(PlayerStateContext psc)
        {
            psc.AnimationManager.SetDashing(false);
            psc.Controller.SetGravity(true);

            if (hitAWall)
            {
                psc.Rb.linearVelocity = Vector3.zero;
                psc.Rb.angularVelocity = Vector3.zero;
            }
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            HandleAnimation(psc);
            
            if (psc.Controller.IsFacingWall() ||  IsHeadingIntoSteepSlope(psc))
            {
                psc.Rb.linearVelocity = Vector3.zero;
                psc.Rb.angularVelocity = Vector3.zero;
                hitAWall = true;
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

            while (elapsed < psc.PlayerData.DashDuration)
            {

                psc.Rb.linearVelocity = dashDirection * psc.PlayerData.DashSpeed;
        
                
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
        }
        
         
        private bool IsHeadingIntoSteepSlope(PlayerStateContext psc)
        {
            Vector3 dashDir = psc.PlayerTransform.forward;
            float checkDist = 0.5f;

            
            Vector3 feetPos = psc.PlayerTransform.position 
                              - Vector3.up * (psc.PlayerData.PlayerHeight / 2f - 0.1f);
            
            #if UNITY_EDITOR
            Debug.DrawRay(feetPos,dashDir*checkDist,Color.coral);
            #endif
            
            if (!Physics.Raycast(feetPos, dashDir, out RaycastHit hit, checkDist, psc.PlayerData.GroundMask))
                return false;

            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
            return slopeAngle > psc.PlayerData.MaxSlopeAngle;  
        }
    }
   
}