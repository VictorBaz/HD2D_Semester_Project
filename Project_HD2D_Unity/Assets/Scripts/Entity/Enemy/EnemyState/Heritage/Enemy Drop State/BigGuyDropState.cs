using System.Collections;
using UnityEngine;

public class BigGuyDropState : EnemyDropState
{
        private Coroutine shockwaveCoroutine;
        
        protected override void LandingSequence(EnemyContext actx)
        { 
                base.LandingSequence(actx);
                
                if (shockwaveCoroutine != null) actx.Manager.StopCoroutine(shockwaveCoroutine);
                ExecuteShockwave(actx);
        }
        

        private void ExecuteShockwave(EnemyContext actx)
        {
                CameraEvents.CameraShake();
                actx.AnimManager.ToggleAttackColliderEnemy(true); 
                shockwaveCoroutine = actx.Manager.StartCoroutine(DisableHitboxLate(actx, actx.Data.ShockwaveActiveDuration));
                actx.VfxManager.TriggerAttackVfx();
        }

        private IEnumerator DisableHitboxLate(EnemyContext actx, float delay)
        {
                yield return new WaitForSeconds(delay);
                actx.AnimManager.ToggleAttackColliderEnemy(false);
        }
}