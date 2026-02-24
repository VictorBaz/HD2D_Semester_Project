using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

[System.Serializable]
public class AiAttack : AiState
{
    public float AttackCooldown = 2f;
    private bool isAttacking = false;
    private Coroutine attackRoutine;

    public override void EnterState(AiBehavior core) 
    { 
        core.movement.StopMovement(); 
    }

    public override void UpdateState(AiBehavior core)
    {
        if (core.targetPlayer == null) 
        { 
            core.ChangeState(core.patrolState); 
            return; 
        }
        
        if (!core.isPlayerInAttackRange)
        {
            core.ChangeState(core.chaseState);
            return;
        }
        
        if (!isAttacking)
        {
            attackRoutine = core.StartCoroutine(AttackSequence(core.targetPlayer.GetComponent<Rigidbody>()));
        }
    }

    public override void ExitState(AiBehavior core) 
    { 
        if (attackRoutine != null)
        {
            core.StopCoroutine(attackRoutine);
            isAttacking = false;
        }
    }

    private IEnumerator AttackSequence(Rigidbody rb)
    {
        isAttacking = true;
        Debug.Log("L'ennemi lance une attaque !");
        
        rb.AddForce(new Vector3(100,100,100),ForceMode.Impulse);
        
        yield return new WaitForSeconds(AttackCooldown); 
        isAttacking = false;
    }
}