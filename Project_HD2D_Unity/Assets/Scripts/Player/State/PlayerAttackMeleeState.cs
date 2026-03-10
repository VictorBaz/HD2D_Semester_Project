using System.Collections;
using UnityEngine;

namespace Player.State
{
    public class PlayerAttackMeleeState : PlayerBaseState
    {
        private Vector3 velocityStock;
        
        public override void EnterState(PlayerStateContext psc)
        {
            velocityStock = psc.Rb.linearVelocity;
            psc.Controller.OnAttackMelee?.Invoke();
            psc.Controller.RunRoutine(AttackMeleeIe(psc));
        }

        public override void ExitState(PlayerStateContext psc){}

        public override void UpdateState(PlayerStateContext psc){}

        public override void FixedUpdateState(PlayerStateContext psc){}
        
        public override bool CanShoot => false;
        
        public override string Name => "Attack Melee";

        public override bool CanMove { get; } = false;


        private IEnumerator AttackMeleeIe(PlayerStateContext psc)
        {
            
            float dashDuration = psc.PlayerData.DashDuration;
            float elapsed = 0f;

            while (elapsed < dashDuration)
            {
                psc.Rb.linearVelocity = Vector3.Lerp(
                    psc.PlayerTransform.forward * psc.PlayerData.DashSpeed,
                    velocityStock,
                    elapsed / dashDuration);

                elapsed += Time.deltaTime;
                yield return null;
            }
            
            yield return new WaitForSeconds(
                psc.PlayerData.GetAttackClipLength() - dashDuration);
            
            psc.StateMachine.TransitionTo(new PlayerLocomotionState());
        }
    }
}