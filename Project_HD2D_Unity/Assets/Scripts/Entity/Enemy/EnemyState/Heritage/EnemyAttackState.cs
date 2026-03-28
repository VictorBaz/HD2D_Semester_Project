using System.Collections;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    private Coroutine attackRoutine;

    private bool isPreparingAttack = false;
    
    private bool isCooldown = false;
    

    public override string Name => "Attacking";

    public override void EnterState(EnemyContext actx)
    {
        actx.Manager.ApplyMovementMode(false);
        actx.StopAgent(); 
        attackRoutine = null;
        isPreparingAttack = false;
        isCooldown = false;
        CanBeParry = false;
    }

    public override void UpdateState(EnemyContext actx)
    {
        if (actx.Target == null) 
        { 
            actx.TransitionTo(actx.Manager.SearchState); 
            return; 
        }

        if (isPreparingAttack || isCooldown || attackRoutine == null)
        {
            RotateTowardsTarget(actx);
        }

        if (attackRoutine != null) return;
        if (isCooldown) return;

        if (!actx.IsPlayerInAttackRange)
        {
            actx.TransitionTo(actx.Manager.ChaseState);
            return;
        }

        attackRoutine = actx.Manager.StartCoroutine(AttackSequence(actx));
    }

    private IEnumerator AttackSequence(EnemyContext actx)
    {
        var data = actx.Data;
        isPreparingAttack = true;
        
        yield return new WaitForSeconds(data.AnticipationTime);
        
        isPreparingAttack = false;
        actx.AnimManager.TriggerAttack(); 
        CanBeParry = true;

        yield return new WaitForFixedUpdate();
        
        actx.AnimManager.ToggleAttackCollider(true);

        
        float elapsed = 0f;
        Vector3 strikeDir = actx.Manager.transform.forward;
        float activePhaseDuration = Mathf.Max(data.AttackDashDuration, data.HitboxActiveDuration);

        while (elapsed < activePhaseDuration)
        {
            elapsed += Time.deltaTime;
            if (elapsed <= data.AttackDashDuration)
                actx.Manager.transform.position += strikeDir * data.AttackDashSpeed * Time.deltaTime;

            if (elapsed >= data.HitboxActiveDuration)
                actx.AnimManager.ToggleAttackCollider(false);

            yield return null;
        }

        actx.AnimManager.ToggleAttackCollider(false);
        yield return new WaitForFixedUpdate();        
        CanBeParry = false;
        
        
        isCooldown = true;

        yield return new WaitForSeconds(data.AttackCooldown);

        attackRoutine = null;
        
        isCooldown = false;  
    }

    private void RotateTowardsTarget(EnemyContext actx)
    {
        if (actx.Target == null) return;
        
        Vector3 lookDir = (actx.Target.transform.position - actx.Manager.transform.position).normalized;
        
        lookDir.y = 0;
        
        if (lookDir != Vector3.zero)
        {
            actx.Manager.transform.rotation = Quaternion.Slerp(
                actx.Manager.transform.rotation, 
                Quaternion.LookRotation(lookDir), 
                Time.deltaTime * 5f 
            );
        }
    }

    public override void ExitState(EnemyContext actx) 
    { 
        if (attackRoutine != null) 
            actx.Manager.StopCoroutine(attackRoutine);
        
        actx.AnimManager.ToggleAttackCollider(false);
        isPreparingAttack = false;
        isCooldown = false; 
        
        CanBeParry = false;

        if (actx.Agent.isActiveAndEnabled) 
            actx.Agent.isStopped = false;
    }

    public override bool CanMove => attackRoutine != null;
}