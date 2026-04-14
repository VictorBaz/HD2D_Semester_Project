using System.Collections;
using UnityEngine;

namespace Player.State
{
    public class PlayerAttackState : PlayerBaseState
    {
        #region Variables

        public override string Name   => "Attack Melee";
        public override bool   CanMove => true;

        private bool      bufferNextAttack;
        private bool      bufferWindowOpen;
        private int       comboIndex;
        private Coroutine currentAttackRoutine;

        #endregion

        #region Base State Methods

        public override void EnterState(PlayerStateContext psc)
        {
            comboIndex       = 0;
            bufferNextAttack = false;

            psc.Controller.SetGravity(false);
            StartAttackSequence(psc);
        }

        public override void ExitState(PlayerStateContext psc)
        {
            if (currentAttackRoutine != null)
                psc.Controller.StopCoroutine(currentAttackRoutine);

            bufferWindowOpen = false;
            bufferNextAttack = false;

            psc.Controller.AttackOff();
            psc.Controller.SetGravity(true);
            psc.AnimationManager.ExitAttack();
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            HandleAnimation(psc);
            HandleMovement(psc);
        }

        public override void FixedUpdateState(PlayerStateContext psc)
        {
            HandlePhysics(psc, 0.45f);
        }

        public void BufferAttack()
        {
            if (bufferWindowOpen)
                bufferNextAttack = true;
        }

        #endregion
        

        private void StartAttackSequence(PlayerStateContext psc)
        {
            if (currentAttackRoutine != null)
                psc.Controller.StopCoroutine(currentAttackRoutine);

            psc.AnimationManager.SetAttackState(true, comboIndex);
            currentAttackRoutine = psc.Controller.RunRoutine(AttackMeleeIe(psc));
        }

        private void RotateTowardsInput(PlayerStateContext psc)
        {
            CalculateTargetDirection(psc);

            if (psc.TargetDirection.magnitude > 0.1f)
                psc.PlayerTransform.forward = psc.TargetDirection;
        }

        private IEnumerator AttackMeleeIe(PlayerStateContext psc)
        {
            CombatHitData hit            = psc.PlayerData.ComboHits[comboIndex];
            float         animLength     = psc.PlayerData.GetAttackClipLength(comboIndex);
            Vector3       dashDir        = psc.PlayerTransform.forward;
            bool          hitboxIsActive = false;
            float         elapsed        = 0f;

            bufferWindowOpen = true;

            psc.VfxManager.PlayFxCombo(comboIndex);
            
            float actionDuration = Mathf.Max(
                animLength,
                hit.DashStartOffset    + hit.DashDuration,
                hit.HitboxStartOffset  + hit.HitboxActiveDuration);

            while (elapsed < actionDuration)
            {
                elapsed += Time.deltaTime;

                UpdateDashVelocity(psc, hit, dashDir, elapsed);
                UpdateHitbox(psc, hit, elapsed, ref hitboxIsActive);

                yield return null;
            }

            psc.Rb.linearVelocity = Vector3.zero;
            psc.Controller.AttackOff();

            yield return new WaitForSeconds(0.1f);
            bufferWindowOpen = false;
            ResolveCombo(psc);
        }

        private void UpdateDashVelocity(PlayerStateContext psc, CombatHitData hit, Vector3 dashDir, float elapsed)
        {
            bool inDashWindow = elapsed >= hit.DashStartOffset && elapsed <= hit.DashStartOffset + hit.DashDuration;

            if (inDashWindow)
            {
                float t = (elapsed - hit.DashStartOffset) / hit.DashDuration;
                psc.Rb.linearVelocity = Vector3.Lerp(dashDir * hit.DashSpeed, Vector3.zero, t);
            }
            else
            {
                psc.Rb.linearVelocity = Vector3.zero;
            }
        }

        private void UpdateHitbox(PlayerStateContext psc, CombatHitData hit, float elapsed, ref bool hitboxIsActive)
        {
            bool shouldBeActive = elapsed >= hit.HitboxStartOffset && elapsed <= hit.HitboxStartOffset + hit.HitboxActiveDuration;

            if (shouldBeActive && !hitboxIsActive)
            {
                psc.Controller.AttackOn();
                hitboxIsActive = true;
            }
            else if (!shouldBeActive && hitboxIsActive)
            {
                psc.Controller.AttackOff();
                hitboxIsActive = false;
            }
        }

        private void ResolveCombo(PlayerStateContext psc)
        {
            if (bufferNextAttack && comboIndex < psc.PlayerData.ComboHits.Length - 1)
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