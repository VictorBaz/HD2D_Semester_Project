using UnityEngine;

public class PlayerBumpState : PlayerInAirBase
{
    public override string Name => "Bump";

    public override void EnterState(PlayerStateContext psc)
    {
        psc.Controller.SetGravity(false);
        psc.Controller.SetJumping(true);
        
        psc.AnimationManager.TriggerJump();
        psc.AnimationManager.SetFalling(false);
    }

    public override void ExitState(PlayerStateContext psc)
    {
        
    }

    public override void UpdateState(PlayerStateContext psc)
    {
        if (psc.Rb.linearVelocity.y <= psc.PlayerData.FallAndJumpThreshold)
        {
            psc.StateMachine.TransitionTo(psc.StateMachine.FallState);
            return;
        }

        HandleAirMovement(psc);
    }

    public override void FixedUpdateState(PlayerStateContext psc)
    {
        AirControl(psc);
        psc.Rb.AddForce(Vector3.down * Physics.gravity.magnitude, ForceMode.Acceleration);
    }
}