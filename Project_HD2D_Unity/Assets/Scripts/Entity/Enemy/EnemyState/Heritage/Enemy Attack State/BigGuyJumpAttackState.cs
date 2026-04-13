using System.Collections;
using UnityEngine;

public class BigGuyJumpAttackState : EnemyAttackState
{
    
    public override void ExitState(EnemyContext actx)
    {
        base.ExitState(actx);
        actx.AnimManager.ToggleRepulsiveCollider(false);
        actx.AnimManager.ToggleAttackCollider(false);
    }

    

    protected override IEnumerator AttackSequence(EnemyContext actx)
    {
        var data = actx.Data;
        
        //TODO ANTICIPATION TIME
        yield return new WaitForSeconds(0.5f);
        
        actx.Manager.ApplyMovementMode(true);
        
        actx.Rb.constraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        
        CanBeParry = false;

        actx.AnimManager.ToggleRepulsiveCollider(true);
        
        Vector3 jumpDirection = Vector3.up * data.AttackJumpForce;
        
        actx.Rb.AddForce(jumpDirection, ForceMode.Impulse);
        
        yield return new WaitForSeconds(0.3f);

        while (!actx.Manager.IsGrounded(actx.Data.GroundDetectionDistance,actx.Data.NavMeshSampleMargin))
        {
            yield return null;
        }
        
        actx.AnimManager.ToggleRepulsiveCollider(false);

        yield return new WaitForEndOfFrame();
        
        actx.Rb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);
        
        ExecuteShockwave(actx);

        canTakeDamage = true;
        
        actx.Rb.linearVelocity = Vector3.zero; 
        actx.Rb.angularVelocity = Vector3.zero;
        
        yield return new WaitForSeconds(data.LandingStunDuration); 
        
        actx.Manager.ApplyMovementMode(false);
        
        yield return new WaitForEndOfFrame();

        isCooldown = true;
        yield return new WaitForSeconds(data.AttackCooldown);
        isCooldown = false;
        attackRoutine = null;
    }

    private void ExecuteShockwave(EnemyContext actx)
    {
        CameraEvents.CameraShake();
        actx.AnimManager.ToggleAttackCollider(true); 
        actx.Manager.StartCoroutine(DisableHitboxLate(actx, actx.Data.ShockwaveActiveDuration));
    }

    private IEnumerator DisableHitboxLate(EnemyContext actx, float delay)
    {
        yield return new WaitForSeconds(delay);
        actx.AnimManager.ToggleAttackCollider(false);
    }
}