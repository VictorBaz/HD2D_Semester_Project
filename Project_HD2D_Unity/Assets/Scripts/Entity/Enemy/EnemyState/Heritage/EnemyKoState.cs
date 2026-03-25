using UnityEngine;

public class EnemyKoState : EnemyBaseState
{
    private float koTimer;

    public override string Name => "K-O";

    public override void EnterState(AiContext actx)
    {
        actx.Behavior.ApplyMovementMode(true); 
        actx.Rb.isKinematic = true; 

        actx.AnimManager.SetKO(true);

        if (koTimer <= 0)
        {
            koTimer = actx.Data.KoTime;
        }
    }

    public override void UpdateState(AiContext actx)
    {
        koTimer -= Time.deltaTime;

        if (actx.Behavior.KoSlider != null)
        {
            actx.Behavior.KoSlider.value = (koTimer / actx.Data.KoTime) * actx.Data.MaxKo;
        }

        if (koTimer <= 0)
        {
            actx.Data.ResetKo();
        
            DetermineNextState(actx);
        }
    }

    private void DetermineNextState(AiContext actx)
    {
        if (actx.Behavior.IsCarry())
        {
            actx.Behavior.Eject(true); 
            return; 
        }

        Vector3 rayOrigin = actx.Behavior.transform.position + Vector3.up * 0.5f;
    
        bool isTouchingGround = Physics.Raycast(rayOrigin, Vector3.down, 0.7f,actx.LayerMaskEnemy);

        if (!isTouchingGround)
        {
            actx.TransitionTo(actx.Behavior.DropState);
        }
        else
        {
            actx.TransitionTo(actx.Behavior.GoToSpawnState);
        }
    }
    

    public override void ExitState(AiContext actx)
    {
        actx.AnimManager.SetKO(false);
    }

    public override bool CanAttack => false;
    public override bool CanMove => false;
    public override bool CanTakeDamage => false;
}