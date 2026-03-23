using System.Collections;
using UnityEngine;

public class AiAttack : AiState
{
    #region Private Variables
    private bool isExecutingSequence = false;
    private Coroutine attackRoutine;
    #endregion

    public override string Name => "Attacking";

    public override void EnterState(AiContext actx) 
    { 
        if (actx.Agent.isActiveAndEnabled)
            actx.Agent.isStopped = true;

        isExecutingSequence = false;
    }

    public override void UpdateState(AiContext actx)
    {
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

        if (isExecutingSequence) return;

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
        
        attackRoutine = actx.Behavior.StartCoroutine(AttackSequence(actx));
    }

    public override void ExitState(AiContext actx) 
    { 
        if (attackRoutine != null) actx.Behavior.StopCoroutine(attackRoutine);
        
        if (actx.Agent.isActiveAndEnabled)
            actx.Agent.isStopped = false;

        isExecutingSequence = false;
    }

    private IEnumerator AttackSequence(AiContext actx)
    {
        isExecutingSequence = true;

        yield return new WaitForSeconds(actx.Data.AnticipationTime);

        if (actx.Target != null && actx.IsPlayerInAttackRange)
        {
            PlayerManager player = actx.Target.GetComponentInParent<PlayerManager>();

            if (player != null)
            {
                player.TransitionTo(player.HitState);
                Debug.Log($"[IA] Touche le joueur !");
            }
            else
            {
                Debug.LogWarning($"[IA] Cible détectée ({actx.Target.name}), mais aucun PlayerManager trouvé !");
            }
        }
    
        yield return new WaitForSeconds(actx.Data.AttackCooldown); 
    
        isExecutingSequence = false;
        attackRoutine = null;
    }
}