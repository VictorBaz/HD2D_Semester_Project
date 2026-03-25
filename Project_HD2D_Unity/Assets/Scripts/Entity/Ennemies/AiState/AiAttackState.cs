using System.Collections;
using UnityEngine;

public class AiAttackState : AiState
{
    private Coroutine attackRoutine;

    private bool isPreparingAttack = false;
    
    private bool isCooldown = false;
    

    public override string Name => "Attacking";

    public override void EnterState(AiContext actx)
    {
        actx.Behavior.ApplyMovementMode(false);
        actx.StopAgent(); 
        attackRoutine = null;
        isPreparingAttack = false;
        isCooldown = false;
        CanBeParry = false;
    }

    public override void UpdateState(AiContext actx)
    {
        if (actx.Target == null) 
        { 
            actx.TransitionTo(actx.Behavior.SearchState); 
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
            actx.TransitionTo(actx.Behavior.ChaseState);
            return;
        }

        attackRoutine = actx.Behavior.StartCoroutine(AttackSequence(actx));
    }

    private IEnumerator AttackSequence(AiContext actx)
    {
        var data = actx.Data;
        isPreparingAttack = true;
        
        yield return new WaitForSeconds(data.AnticipationTime);
        
        isPreparingAttack = false;
        actx.AnimManager.TriggerAttack(); 
        actx.AnimManager.AttackOn();

        CanBeParry = true;
        
        float elapsed = 0f;
        Vector3 strikeDir = actx.Behavior.transform.forward;
        float activePhaseDuration = Mathf.Max(data.AttackDashDuration, data.HitboxActiveDuration);

        while (elapsed < activePhaseDuration)
        {
            elapsed += Time.deltaTime;
            if (elapsed <= data.AttackDashDuration)
                actx.Behavior.transform.position += strikeDir * data.AttackDashSpeed * Time.deltaTime;

            if (elapsed >= data.HitboxActiveDuration)
                actx.AnimManager.AttackOff();

            yield return null;
        }

        CanBeParry = false;
        
        actx.AnimManager.AttackOff();

        isCooldown = true;

        yield return new WaitForSeconds(data.AttackCooldown);

        attackRoutine = null;
        
        isCooldown = false;  
    }

    private void RotateTowardsTarget(AiContext actx)
    {
        if (actx.Target == null) return;
        
        Vector3 lookDir = (actx.Target.transform.position - actx.Behavior.transform.position).normalized;
        
        lookDir.y = 0;
        
        if (lookDir != Vector3.zero)
        {
            actx.Behavior.transform.rotation = Quaternion.Slerp(
                actx.Behavior.transform.rotation, 
                Quaternion.LookRotation(lookDir), 
                Time.deltaTime * 5f 
            );
        }
    }

    public override void ExitState(AiContext actx) 
    { 
        if (attackRoutine != null) 
            actx.Behavior.StopCoroutine(attackRoutine);
        
        actx.AnimManager.AttackOff();
        isPreparingAttack = false;
        isCooldown = false; 
        
        CanBeParry = false;

        if (actx.Agent.isActiveAndEnabled) 
            actx.Agent.isStopped = false;
    }

    public override bool CanMove => attackRoutine != null;
}