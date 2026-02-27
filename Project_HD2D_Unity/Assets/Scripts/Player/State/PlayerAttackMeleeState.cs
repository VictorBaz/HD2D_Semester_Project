using System.Collections;
using UnityEngine;

namespace Player.State
{
    public class PlayerAttackMeleeState : PlayerBaseState
    {
        public override void EnterState(PlayerStateContext psc)
        {
            psc.Controller.OnAttackMelee?.Invoke();
            psc.Controller.RunRoutine(AttackMeleeIe(psc));
        }

        public override void ExitState(PlayerStateContext psc)
        {
       
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            
        }

        public override void FixedUpdateState(PlayerStateContext psc)
        {
        
        }
        
        
        private IEnumerator AttackMeleeIe(PlayerStateContext psc)
        {
            
            float dashDuration = 0.35f;
            float elapsed = 0f;

            while (elapsed < dashDuration)
            {
                psc.Rb.linearVelocity = Vector3.Lerp(
                    psc.PlayerTransform.forward * psc.PlayerData.DashSpeed,
                    Vector3.zero,
                    elapsed / dashDuration);

                elapsed += Time.deltaTime;
                yield return null;
            }

            psc.Controller.ToggleFixPlayerPosition(true);
            yield return new WaitForSeconds(
                psc.PlayerData.GetLengthOfClip(psc.PlayerData.AttackClip) - dashDuration);

            psc.Controller.ToggleFixPlayerPosition(false);
            
            psc.StateMachine.TransitionTo(new PlayerLocomotionState());
        }
        
        public override bool CanShoot => false;
    }
}