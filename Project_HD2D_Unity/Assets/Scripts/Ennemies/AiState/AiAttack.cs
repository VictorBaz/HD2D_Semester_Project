using System.Collections;
using UnityEngine;

public class AiAttack : AiState
{
    private float attackTimer;
    private float attackCooldown;
    
    public override string Name => "Attacking";

    public override void EnterState(AiContext actx)
    {
        attackTimer = 0f;
        attackCooldown = actx.Data.AttackCooldown;
        
        if (actx.Agent.isActiveAndEnabled)
            actx.Agent.isStopped = CanMove;
        
    }

    public override void UpdateState(AiContext actx)
    {
        
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            actx.AnimManager.EnterAttack();
            attackTimer = attackCooldown; 
        }
        
        if (actx.Target != null)
        {
            Vector3 lookDir = (actx.Target.transform.position - actx.Behavior.transform.position).normalized;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                actx.Behavior.transform.rotation = Quaternion.Slerp(
                    actx.Behavior.transform.rotation, 
                    Quaternion.LookRotation(lookDir), 
                    Time.deltaTime * 10f
                );
            }
        }
        

        if (actx.Target == null) 
        { 
            actx.TransitionTo(actx.Behavior.SearchState);
            return; 
        }

        if (!actx.IsPlayerInAttackRange)
        {
            actx.TransitionTo(actx.Behavior.ChaseState);
            return;
        }
        
    }

    public override void ExitState(AiContext actx) 
    { 
        
        if (actx.Agent.isActiveAndEnabled)
            actx.Agent.isStopped = false;

        actx.AnimManager.ExitAttack();
    }
    
    public override bool CanMove => false;
    public override bool CanTakeDamage => false;
}