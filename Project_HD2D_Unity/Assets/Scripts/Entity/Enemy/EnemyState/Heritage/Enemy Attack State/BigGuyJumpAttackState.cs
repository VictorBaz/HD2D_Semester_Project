using System.Collections;
using UnityEngine;

public class BigGuyJumpAttackState : EnemyAttackState
{
    private float chargedTime;
    
    public override void EnterState(EnemyContext actx)
    {
        base.EnterState(actx);
        actx.AnimManager.UpdateMovement(GameConstants.ANIM_MAGNITUDE_IDLE);
        actx.AnimManager.ToggleRepulsiveCollider(true);

        chargedTime = actx.Data.GetAnimationCLipLengthChargeAttack();
    }

    public override void ExitState(EnemyContext actx)
    {
        base.ExitState(actx);
        actx.AnimManager.ToggleAttackCollider(false);
        
        actx.Rb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);
        actx.AnimManager.Animator.speed = 1f;
        
        canTakeDamage = true;
        
        if (shaderRoutine != null) actx.Manager.StopCoroutine(shaderRoutine);
        actx.SetVisualParam(GameConstants.PARAM_SHEEP_SHADER_NAME,0,GameConstants.INDEX_MATERIAL_PULSE);
    }
    


    protected override IEnumerator AttackSequence(EnemyContext actx)
    {
        var data = actx.Data;
        
        actx.AnimManager.TriggerCharge();
        
        if (shaderRoutine != null) actx.Manager.StopCoroutine(shaderRoutine);
        shaderRoutine = actx.Manager.StartCoroutine(ShaderPulseOn(actx));
        
        yield return new WaitForSeconds(chargedTime);
        
        canTakeDamage = false;
        
        actx.Manager.ApplyMovementMode(true);
        
        actx.Rb.constraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        
        CanBeParry = false;
        
        actx.AnimManager.TriggerAttack();
        
        if (shaderRoutine != null) actx.Manager.StopCoroutine(shaderRoutine);
        shaderRoutine = actx.Manager.StartCoroutine(ShaderPulseOff(actx));
        
        Vector3 jumpDirection = Vector3.up * data.AttackJumpForce;
        
        actx.Rb.AddForce(jumpDirection, ForceMode.Impulse);
        
        yield return new WaitForSeconds(0.3f);

        while (!actx.Manager.IsGrounded(actx.Data.GroundDetectionDistance,actx.Data.NavMeshSampleMargin))
        {
            yield return null;
        }
        

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
        actx.VfxManager.TriggerAttackVfx();
    }

    private IEnumerator DisableHitboxLate(EnemyContext actx, float delay)
    {
        yield return new WaitForSeconds(delay);
        actx.AnimManager.ToggleAttackCollider(false);
    }
}