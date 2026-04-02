using System.Collections;
using Manager;
using UnityEngine;

public class BigGuyJumpAttackState : EnemyAttackState
{
    private bool canTakeDamage;

    public override void EnterState(EnemyContext actx)
    {
        canTakeDamage = false;
        base.EnterState(actx);
    }

    public override void ExitState(EnemyContext actx)
    {
        base.ExitState(actx);
        canTakeDamage = true;
    }

    public override bool CanTakeDamage => canTakeDamage;

    protected override IEnumerator AttackSequence(EnemyContext actx)
    {
        var data = actx.Data;

        yield return new WaitForSeconds(data.AnticipationTime);
        
        actx.Manager.ApplyMovementMode(true);
        
        actx.Rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        CanBeParry = false;

        Vector3 jumpDirection = Vector3.up * data.AttackJumpForce;
        actx.Rb.AddForce(jumpDirection, ForceMode.Impulse);
        
        yield return new WaitForSeconds(0.3f);

        while (!actx.Manager.IsGrounded(actx.Data.GroundDetectionDistance,actx.Data.NavMeshSampleMargin))
        {
            yield return null;
        }
        
        actx.Rb.constraints &= ~(RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ);
        
        ExecuteShockwave(actx);

        yield return new WaitForSeconds(data.LandingStunDuration); 
        
        
        actx.Manager.ApplyMovementMode(false);
        
        isCooldown = true;
        yield return new WaitForSeconds(data.AttackCooldown);
        isCooldown = false;
        attackRoutine = null;
    }

    private void ExecuteShockwave(EnemyContext actx)
    {
        EventManager.CameraShake();
        actx.AnimManager.ToggleAttackCollider(true); 
        actx.Manager.StartCoroutine(DisableHitboxLate(actx, actx.Data.ShockwaveActiveDuration));
    }

    private IEnumerator DisableHitboxLate(EnemyContext actx, float delay)
    {
        yield return new WaitForSeconds(delay);
        actx.AnimManager.ToggleAttackCollider(false);
    }
}