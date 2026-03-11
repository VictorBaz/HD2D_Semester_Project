using System.Collections;
using Manager;
using UnityEngine;

namespace Player.State
{
    public class PlayerAttackMeleeState : PlayerBaseState
    {
        public override bool CanShoot => false;
        public override bool CanMove  => false;
        public override string Name   => "Attack Melee";

        public override void EnterState(PlayerStateContext psc)
        {
            psc.Controller.OnAttackMelee?.Invoke();
            psc.Controller.RunRoutine(AttackMeleeIe(psc));
        }

        public override void ExitState(PlayerStateContext psc) { }

        public override void UpdateState(PlayerStateContext psc)
        {
            HandleAnimation(psc);
        }

        public override void FixedUpdateState(PlayerStateContext psc) { }

        private IEnumerator AttackMeleeIe(PlayerStateContext psc)
        {
            float dashDuration = psc.PlayerData.DashDurationAttack;
            float elapsed      = 0f;
            while (elapsed < dashDuration)
            {
                psc.Rb.linearVelocity = Vector3.Lerp(
                    psc.PlayerTransform.forward * psc.PlayerData.DashSpeedAttack,
                    Vector3.zero,
                    elapsed / dashDuration);

                elapsed += Time.deltaTime;
                yield return null;
            }
            

            psc.Rb.linearVelocity = Vector3.zero;
            yield return new WaitForSeconds(
                psc.PlayerData.GetAttackClipLength() - dashDuration);
            Debug.Log("End Attack");
            psc.StateMachine.TransitionTo(psc.StateMachine.LocomotionState);
        }
    }
}