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
        
        if (core.isPlayerInAttackRange && core.target != null)
        {
            core.ChangeState(core.attackState);
        }
        
        if(core.isPlayerInViewRange && core.target != null)
        {
            core.ChangeState(core.chaseState); 
        }

        if (core.target != null)
        {
            core.ChangeState(core.searchState);
        }
    }
    
    public override string Name => "K-O";
}