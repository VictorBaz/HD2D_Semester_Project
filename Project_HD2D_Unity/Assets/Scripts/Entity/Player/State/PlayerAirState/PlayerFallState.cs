using UnityEngine;

public class PlayerFallState : PlayerInAirBase
{
    private float fallStartTime;

    public override string Name => "Fall";

    public override void EnterState(PlayerStateContext psc)
    {
        fallStartTime = Time.time;
        psc.Controller.SetGravity(true);
        psc.Controller.SetJumping(false);
        psc.AnimationManager.SetFalling(true);
    }

    public override void UpdateState(PlayerStateContext psc)
    {
        if (psc.Controller.IsGrounded)
        {
            psc.StateMachine.TransitionTo(psc.StateMachine.LandingState);
            return;
        }

        HandleAirMovement(psc);
    }

    public override void FixedUpdateState(PlayerStateContext psc)
    {
        AirControl(psc);
        
        float timeFalling = Time.time - fallStartTime;
        float gravityRatio = Mathf.Clamp01(timeFalling / psc.PlayerData.MaxGravityTime);
        float currentMultiplier = Mathf.Lerp(1f, psc.PlayerData.GravityMultiplier, gravityRatio);
            
        psc.Rb.AddForce(Vector3.down * currentMultiplier * Physics.gravity.magnitude, ForceMode.Acceleration);
    }

    public override void ExitState(PlayerStateContext psc)
    {
        psc.AnimationManager.SetFalling(false);
    }
}