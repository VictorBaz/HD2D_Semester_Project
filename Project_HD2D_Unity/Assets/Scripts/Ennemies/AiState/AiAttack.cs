using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AiAttack : AiState
{
    public float AttackCooldown = 1f;
    public float KnockbackStrength = 15f;
    public float AnticipationTime = 0.4f;
    
    private bool isExecutingSequence = false;
    private Coroutine attackRoutine;

    public override void EnterState(AiBehavior core) 
    { 
        core.movement.StopMovement();
        isExecutingSequence = false;
    }

    public override void UpdateState(AiBehavior core)
    {
        if (core.target != null)
        {
            Vector3 lookDir = (core.target.transform.position - core.transform.position).normalized;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                core.transform.rotation = Quaternion.Slerp(core.transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);
            }
        }

        if (isExecutingSequence) return;

        if (core.target == null) 
        { 
            core.ChangeState(core.searchState);
            return; 
        }

        if (!core.isPlayerInAttackRange)
        {
            core.ChangeState(core.chaseState);
            return;
        }
        
        attackRoutine = core.StartCoroutine(AttackSequence(core));
    }

    public override void ExitState(AiBehavior core) 
    { 
        if (attackRoutine != null) core.StopCoroutine(attackRoutine);
        isExecutingSequence = false;
    }

    private IEnumerator AttackSequence(AiBehavior core)
    {
        isExecutingSequence = true;

        yield return new WaitForSeconds(AnticipationTime);

        if (core.target != null && core.isPlayerInAttackRange)
        {
            Rigidbody playerRb = core.target.GetComponentInParent<Rigidbody>();

            if (playerRb != null)
            {
                Vector3 knockbackDir = (playerRb.transform.position - core.transform.position).normalized;
                knockbackDir.y = 0.2f;
                playerRb.AddForce(knockbackDir * KnockbackStrength, ForceMode.Impulse);
                Debug.Log("Player has been hit");
            }
            else
            {
                Debug.LogWarning($"Cible trouvée ({core.target.name}), mais aucun Rigidbody trouvé ici ou dans les parents !");
            }
        }
    
        yield return new WaitForSeconds(AttackCooldown); 
    
        isExecutingSequence = false;
        attackRoutine = null;
    }
    
    public override string Name => "Attacking";
}