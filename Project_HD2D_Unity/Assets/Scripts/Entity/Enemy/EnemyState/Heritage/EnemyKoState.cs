using UnityEngine;

public class EnemyKoState : EnemyBaseState
{
    private float koTimer;

    public override string Name => "K-O";

    public override void EnterState(EnemyContext actx)
    {
        actx.Manager.ApplyMovementMode(true); 
        actx.Rb.isKinematic = true; 

        actx.AnimManager.SetKO(true);

        if (koTimer <= 0)
        {
            koTimer = actx.Data.KoTime;
        }
    }

    public override void UpdateState(EnemyContext actx)
    {
        koTimer -= Time.deltaTime;

        if (actx.Manager.KoSlider != null)
        {
            actx.Manager.KoSlider.value = (koTimer / actx.Data.KoTime) * actx.Data.MaxKo;
        }

        if (koTimer <= 0)
        {
            actx.Data.ResetKo();
        
            DetermineNextState(actx);
        }
    }

    private void DetermineNextState(EnemyContext actx)
    {
        if (actx.Manager.IsCarry())
        {
            actx.Manager.Eject(true); 
            return; 
        }

        Vector3 rayOrigin = actx.Manager.transform.position + Vector3.up * 0.5f;
    
        bool isTouchingGround = Physics.Raycast(rayOrigin, Vector3.down, 0.7f,actx.LayerMaskEnemy);

        if (!isTouchingGround)
        {
            actx.TransitionTo(actx.Manager.DropState);
        }
        else
        {
            actx.TransitionTo(actx.Manager.GoToSpawnState);
        }
    }
    

    public override void ExitState(EnemyContext actx)
    {
        actx.AnimManager.SetKO(false);
    }

    public override bool CanAttack => false;
    public override bool CanMove => false;
    public override bool CanTakeDamage => false;
}