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
        private int  comboIndex = 0;

        public override bool CanShoot => false;
        public override bool CanMove => false;
        public override string Name => "Attack Melee";

        #endregion

        #region Base

        public override void EnterState(PlayerStateContext psc)
        {
            bufferNextAttack = false;
            psc.AnimationManager.SetComboIndex(comboIndex);
            psc.Controller.RunRoutine(AttackMeleeIe(psc));
        }

        public override void ExitState(PlayerStateContext psc) { }

        public override void UpdateState(PlayerStateContext psc)
        {
            HandleAnimation(psc);
        }

        public override void FixedUpdateState(PlayerStateContext psc) { }

        #endregion

        #region Public

        public void BufferAttack()
        {
            if (bufferWindowOpen)
                bufferNextAttack = true;
        }

        #endregion

        #region Combo

        private void ResolveCombo(PlayerStateContext psc)
        {
            bool canCombo = bufferNextAttack && comboIndex < psc.PlayerData.ComboHits.Length - 1;

            if (canCombo)
            {
                comboIndex++;
                bufferNextAttack = false;
                psc.AnimationManager.SetComboIndex(comboIndex);
                psc.Controller.RunRoutine(AttackMeleeIe(psc));
            }
            else
            {
                comboIndex = 0;
                psc.StateMachine.TransitionTo(psc.StateMachine.LocomotionState);
            }
        }

        #endregion

        #region Coroutines

        private IEnumerator AttackMeleeIe(PlayerStateContext psc)
        {
            CombatHitData hit = psc.PlayerData.ComboHits[comboIndex];

            yield return DashIe(hit, psc);
            yield return WaitBeforeWindowIe(hit, psc);
            yield return OpenWindowIe(psc);

            ResolveCombo(psc);
        }

        private IEnumerator DashIe(CombatHitData data, PlayerStateContext psc)
        {
            float elapsed = 0f;

            while (elapsed < data.DashDuration)
            {
                psc.Rb.linearVelocity = Vector3.Lerp(
                    psc.PlayerTransform.forward * data.DashSpeed,
                    Vector3.zero,
                    elapsed / data.DashDuration);

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator WaitBeforeWindowIe(CombatHitData hit, PlayerStateContext psc)
        {
            psc.Rb.linearVelocity = Vector3.zero;

            float waitBeforeWindow = psc.PlayerData.GetAttackClipLength(comboIndex)
                                     - hit.DashDuration
                                     - psc.PlayerData.ComboWindow;

            if (waitBeforeWindow > 0f)
                yield return new WaitForSeconds(waitBeforeWindow);
        }

        private IEnumerator OpenWindowIe(PlayerStateContext psc)
        {
            bufferWindowOpen = true;
            yield return new WaitForSeconds(psc.PlayerData.ComboWindow);
            bufferWindowOpen = false;
        }

        #endregion
    }
}