using System.Collections;
using Manager;
using UnityEngine;

namespace Player.State
{
    public class PlayerAttackMeleeState : PlayerBaseState
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
        }

        public override void ExitState(PlayerStateContext psc) 
        {
            if (currentAttackRoutine != null)
                psc.Controller.StopCoroutine(currentAttackRoutine);
            
            bufferWindowOpen = false;
            bufferNextAttack = false;
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

            psc.AnimationManager.SetComboIndex(comboIndex);
            currentAttackRoutine = psc.Controller.RunRoutine(AttackMeleeIe(psc));
        }

        private void RotateTowardsInput(PlayerStateContext psc)
        {
            CalculateTargetDirection(psc);
            
            if (targetDirection.magnitude > 0.1f)
            {
                psc.PlayerTransform.forward = targetDirection;
            }
        }

        private IEnumerator AttackMeleeIe(PlayerStateContext psc)
        {
            CombatHitData hit = psc.PlayerData.ComboHits[comboIndex];

            yield return DashIe(hit, psc);

            yield return new WaitUntil(() => psc.AnimationManager.IsInAttackAnimation());

            bufferWindowOpen = true;

            float clipLength = psc.AnimationManager.GetCurrentAnimatorStateInfo(0).length;
            
            yield return new WaitForSeconds(clipLength * 0.7f);

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
                psc.StateMachine.TransitionTo(psc.StateMachine.LocomotionState);
            }
        }

        private IEnumerator DashIe(CombatHitData data, PlayerStateContext psc)
        {
            float elapsed = 0f;
            Vector3 dashDir = psc.PlayerTransform.forward; 

            while (elapsed < data.DashDuration)
            {
                psc.Rb.linearVelocity = Vector3.Lerp(
                    dashDir * data.DashSpeed,
                    Vector3.zero,
                    elapsed / data.DashDuration);

                elapsed += Time.deltaTime;
                yield return null;
            }
            psc.Rb.linearVelocity = Vector3.zero;
        }
    }
}