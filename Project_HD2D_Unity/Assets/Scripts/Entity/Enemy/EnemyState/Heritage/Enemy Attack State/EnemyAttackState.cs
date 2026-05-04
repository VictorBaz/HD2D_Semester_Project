using System.Collections;
using UnityEngine;

public abstract class EnemyAttackState : EnemyBaseState
{
    protected Coroutine attackRoutine;
    protected bool isCooldown;
    protected bool isAnticipationTime;
    
    protected bool canTakeDamage;
    
    protected float chargedTime;
    protected Coroutine shaderRoutine;
    
    public override bool CanTakeDamage => canTakeDamage;
    
    public override string Name => "Attacking";
    public override bool CanMove => attackRoutine != null;

    public override void EnterState(EnemyContext actx)
    {
        actx.StopAgent();
        actx.Manager.ApplyMovementMode(false);

        attackRoutine = null;
        isCooldown = false;
        CanBeParry = false;
        isAnticipationTime = false;
        canTakeDamage = true;
    }

    public override void UpdateState(EnemyContext actx)
    {
        if (!actx.Manager.IsGrounded(actx.Data.GroundDetectionDistance,actx.Data.NavMeshSampleMargin)) return;
        
        if (actx.Target == null)
        {
            actx.TransitionTo(actx.Manager.SearchState);
            return;
        }

        if (isCooldown || attackRoutine == null || isAnticipationTime)
            RotateTowardsTarget(actx);

        if (attackRoutine != null || isCooldown) return;

        if (!actx.IsPlayerInAttackRange)
        {
            actx.TransitionTo(actx.Manager.ChaseState);
            return;
        }

        attackRoutine = actx.Manager.StartCoroutine(AttackSequence(actx));
    }

    protected abstract IEnumerator AttackSequence(EnemyContext actx);

    protected void RotateTowardsTarget(EnemyContext actx)
    {
        if (actx.Target == null) return;
        Vector3 lookDir = (actx.Target.transform.position - actx.Manager.transform.position).normalized;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            actx.Manager.transform.rotation = Quaternion.Slerp(
                actx.Manager.transform.rotation, 
                Quaternion.LookRotation(lookDir), 
                Time.deltaTime * actx.Data.RotationSpeed);
        }
    }

    public override void ExitState(EnemyContext actx)
    {
        if (attackRoutine != null) actx.Manager.StopCoroutine(attackRoutine);
        canTakeDamage = true;
    }
    
    protected IEnumerator ShaderPulseOn(EnemyContext actx) => ShaderSheepUpdateIe
        (actx,chargedTime,GameConstants.PARAM_SHEEP_SHADER_MAX);
    
    protected IEnumerator ShaderPulseOff(EnemyContext actx) => ShaderSheepUpdateIe
        (actx,0.2f,0);
    
    protected IEnumerator ShaderSheepUpdateIe(EnemyContext actx, float time, float targetValue)
    {
        float elapsed = 0f;
    
        float startValue = actx.GetVisualParam(GameConstants.PARAM_SHEEP_SHADER_NAME, GameConstants.INDEX_MATERIAL_PULSE); 

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / time;
        
            float v = Mathf.Lerp(startValue, targetValue, t);
            actx.SetVisualParam(GameConstants.PARAM_SHEEP_SHADER_NAME, v, GameConstants.INDEX_MATERIAL_PULSE);
            yield return null;
        }
    
        actx.SetVisualParam(GameConstants.PARAM_SHEEP_SHADER_NAME, targetValue, GameConstants.INDEX_MATERIAL_PULSE);
    }
}