using UnityEngine;

public class AiPostDrop : AiState
{
    public override string Name => "Post_Drop";
    
    public override void EnterState(AiContext actx) { }

    public override void UpdateState(AiContext actx) { }

    public override void ExitState(AiContext actx) { }
    
    public override bool CanAttack => false;
    
    public override bool CanMove => false;
    
    public override bool CanTakeDamage => false;
}
