using System.Collections;
using UnityEngine;

[System.Serializable]
public class AiKO : AiState
{
    public float KoTime = 5f;
    private Coroutine KoRoutine;
    
    public override void EnterState(AiBehavior core)
    {
        core.movement.StopMovement();
        core.KoSlider.value = 0; 
        KoRoutine = core.StartCoroutine(KoMoment(core));
    }

    public override void UpdateState(AiBehavior core)
    {
        
    }

    public override void ExitState(AiBehavior core)
    {
        if (KoRoutine != null)
        {
            core.StopCoroutine(KoRoutine);
            KoRoutine = null;
        }
    }
    
    private IEnumerator KoMoment(AiBehavior core)
    {
        Debug.Log("L'ennemi est KO...");
        
        yield return new WaitForSeconds(KoTime); 
        
        if (core.isPlayerInAttackRange)
        {
            core.ChangeState(core.attackState);
        }
        else
        {
            core.ChangeState(core.patrolState); 
        }
    }
    
    public override string Name => "K-O";
}