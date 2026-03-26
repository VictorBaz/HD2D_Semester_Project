using System.Collections;
using UnityEngine;

namespace Player.State
{
    public class PlayerAttackState : PlayerBaseState
    {
        #region Variables
        private bool bufferNextAttack = false;
        private bool bufferWindowOpen = false;
        private int comboIndex = 0;
        private Coroutine currentAttackRoutine;

        public override bool CanMove => true; 
        public override string Name => "Attack Melee";
        #endregion

        public override void EnterState(PlayerStateContext psc)
        {
            comboIndex = 0;
            
            bufferNextAttack = false;
            
            StartAttackSequence(psc);
            
            psc.Controller.SetGravity(false);
        }

        public override void ExitState(PlayerStateContext psc) 
        {
            if (currentAttackRoutine != null)
                psc.Controller.StopCoroutine(currentAttackRoutine);
            
            psc.Controller.SetGravity(true);
            
            bufferWindowOpen = false;
            bufferNextAttack = false;
            psc.Controller.AttackOff();
            psc.AnimationManager.ExitAttack();
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            HandlePhysics(psc, 0.2f);
            HandleAnimation(psc);
        }

        public override void FixedUpdateState(PlayerStateContext psc) { }

        public void BufferAttack()
        {
            if (bufferWindowOpen)
                bufferNextAttack = true;
        }

        private void StartAttackSequence(PlayerStateContext psc)
        {
            if (currentAttackRoutine != null)
                psc.Controller.StopCoroutine(currentAttackRoutine);
            RotateTowardsInput(psc);

            psc.AnimationManager.SetAttackState(true,comboIndex);
            currentAttackRoutine = psc.Controller.RunRoutine(AttackMeleeIe(psc));
        }

        private void RotateTowardsInput(PlayerStateContext psc)
        {
            CalculateTargetDirection(psc);
            
            if (psc.TargetDirection.magnitude > 0.1f)
            {
                psc.PlayerTransform.forward = psc.TargetDirection;
            }
        }

        private IEnumerator AttackMeleeIe(PlayerStateContext psc)
        {
            CombatHitData hit = psc.PlayerData.ComboHits[comboIndex];
            psc.AnimationManager.SetAttackState(true,comboIndex);
            bufferWindowOpen = true;

            float elapsed = 0f;
            Vector3 dashDir = psc.PlayerTransform.forward;
            bool hitboxIsActive = false;
            
            float actionDuration = Mathf.Max(
                hit.DashStartOffset + hit.DashDuration, 
                hit.HitboxStartOffset + hit.HitboxActiveDuration
            );

            while (elapsed < actionDuration)
            {
                elapsed += Time.deltaTime;
                
                if (elapsed >= hit.DashStartOffset && elapsed <= (hit.DashStartOffset + hit.DashDuration))
                {
                    
                    float localDashTime = (elapsed - hit.DashStartOffset) / hit.DashDuration;
                    
                    psc.Rb.linearVelocity = Vector3.Lerp(
                        dashDir * hit.DashSpeed,
                        Vector3.zero,
                        localDashTime);
                }
                else
                {
                    
                    psc.Rb.linearVelocity = Vector3.zero;
                }

                bool shouldHitboxBeActive = elapsed >= hit.HitboxStartOffset && 
                                            elapsed <= (hit.HitboxStartOffset + hit.HitboxActiveDuration);

                if (shouldHitboxBeActive && !hitboxIsActive)
                {
                    psc.Controller.AttackOn();
                    hitboxIsActive = true;
                }
                else if (!shouldHitboxBeActive && hitboxIsActive)
                {
                    psc.Controller.AttackOff();
                    hitboxIsActive = false;
                }

                yield return null;
            }

            psc.Rb.linearVelocity = Vector3.zero;
            psc.Controller.AttackOff();
            
            yield return new WaitForSeconds(0.1f); 
            bufferWindowOpen = false;
            ResolveCombo(psc);
        }

        private void ResolveCombo(PlayerStateContext psc)
        {
            bool canCombo = bufferNextAttack && comboIndex < psc.PlayerData.ComboHits.Length - 1;

            if (canCombo)
            {
                comboIndex++;
                bufferNextAttack = false;
                StartAttackSequence(psc);
            }
            else
            {
                DetermineState(psc);
            }
        }

        
    }
}