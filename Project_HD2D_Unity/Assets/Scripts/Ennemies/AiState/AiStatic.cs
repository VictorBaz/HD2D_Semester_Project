using UnityEngine;

[System.Serializable]
public class AiStatic : AiState
{
    public override void EnterState(AiBehavior core)
    {
        
    }
    public override void UpdateState(AiBehavior core)
    {
        if (core.CanSeePlayer()) core.ChangeState(core.chaseState);
    }

    public override void ExitState(AiBehavior core)
    {
        
    }
    
    public override string Name => "Static";
}