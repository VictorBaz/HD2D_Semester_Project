using System;
using System.Collections;
using UnityEngine;

public class EnemySheepAttackState : EnemyAttackState
{
    private const float MULTIPLY_ANTICIPATION_TIME = 2f;
    

    public override void EnterState(EnemyContext actx)
    {
        base.EnterState(actx);
        
        actx.AnimManager.UpdateMovement(GameConstants.ANIM_MAGNITUDE_IDLE);

        chargedTime = actx.Data.GetAnimationCLipLengthChargeAttack() / MULTIPLY_ANTICIPATION_TIME;
        
        actx.AnimManager.ToggleRepulsiveCollider(true);
    }

    protected override IEnumerator AttackSequence(EnemyContext actx)
    {
        canTakeDamage = true;
        
        isAnticipationTime = true;
        var data = actx.Data;
        
        if (shaderRoutine != null) actx.Manager.StopCoroutine(shaderRoutine);
        shaderRoutine = actx.Manager.StartCoroutine(ShaderPulseOn(actx));

        actx.AnimManager.Animator.speed = MULTIPLY_ANTICIPATION_TIME;
        
        actx.AnimManager.TriggerCharge();
        
        yield return new WaitForSeconds(data.GetAnimationCLipLengthChargeAttack() / MULTIPLY_ANTICIPATION_TIME);

        isAnticipationTime = false;
        
        actx.AnimManager.Animator.speed = 1;
        
        actx.AnimManager.TriggerAttack();

        if (shaderRoutine != null) actx.Manager.StopCoroutine(shaderRoutine);
        shaderRoutine = actx.Manager.StartCoroutine(ShaderPulseOff(actx));
        
        canTakeDamage = false;

        CanBeParry = true;
        
        actx.AnimManager.ToggleAttackCollider(true);

        float elapsed = 0f;
        
        Vector3 strikeDir = actx.Manager.transform.forward;
        
        float activePhaseDuration = actx.Data.GetAnimationCLipLengthAttack();

        
        while (elapsed < activePhaseDuration)
        {
            
            elapsed += Time.deltaTime;
            
            if (elapsed <= data.AttackDashDuration)
            {
                Vector3 nextPos = actx.Rb.position + strikeDir * (data.AttackDashSpeed * 0.05f);
                actx.Rb.MovePosition(nextPos);
            }

            if (elapsed >= data.HitboxActiveDuration)
                actx.AnimManager.ToggleAttackCollider(false);

            yield return null;
        }
        
        yield return new WaitForFixedUpdate();
        
        actx.AnimManager.ToggleAttackCollider(false);
        
        yield return new WaitForFixedUpdate();
        
        canTakeDamage = true;
        
        CanBeParry = false;

        isCooldown = true;
        yield return new WaitForSeconds(data.AttackCooldown);
        
        isCooldown = false;
        
        attackRoutine = null;
    }

    public override void ExitState(EnemyContext actx)
    {
        base.ExitState(actx);
        if (shaderRoutine != null) actx.Manager.StopCoroutine(shaderRoutine);
        actx.AnimManager.ToggleAttackCollider(false);
        actx.AnimManager.Animator.speed = 1;
        actx.SetVisualParam(GameConstants.PARAM_SHEEP_SHADER_NAME,0,1);
    }

    
}