using UnityEngine;

public class PlayerJumpState : PlayerInAirBase
{
    private float requiredJumpGravity;
    private float jumpStartTime;

    public override string Name => "Jump";

    public override void EnterState(PlayerStateContext psc)
    {
        jumpStartTime = Time.time;
        psc.JumpReleased = false;
        psc.Controller.SetGravity(false);
        psc.Controller.SetJumping(true);

        float h = psc.PlayerData.JumpHeight;
        float t = psc.PlayerData.JumpDuration;
        requiredJumpGravity = (2f * h) / (t * t);

        if (psc.Rb.linearVelocity.y < 0.1f)
        {
            float jumpVelocity = requiredJumpGravity * t;
            psc.Rb.linearVelocity = new Vector3(psc.Rb.linearVelocity.x, jumpVelocity, psc.Rb.linearVelocity.z);
        }
            
        psc.AnimationManager.TriggerJump();
        psc.AnimationManager.SetFalling(false);
    }

    public override void ExitState(PlayerStateContext psc)
    {
        psc.AnimationManager.ResetJump();
    }

    public override void UpdateState(PlayerStateContext psc)
    {
        if (psc.JumpReleased && psc.Rb.linearVelocity.y > 0)
        {
            psc.Rb.linearVelocity = new Vector3(psc.Rb.linearVelocity.x, psc.Rb.linearVelocity.y * 0.5f, psc.Rb.linearVelocity.z);
            psc.StateMachine.TransitionTo(psc.StateMachine.FallState);
            return;
        }

        if (psc.Rb.linearVelocity.y <= 0.1f)
        {
            psc.StateMachine.TransitionTo(psc.StateMachine.FallState);
            return;
        }

        HandleAirMovement(psc);
    }

    public override void FixedUpdateState(PlayerStateContext psc)
    {
        AirControl(psc);
        
        psc.Rb.AddForce(Vector3.down * requiredJumpGravity, ForceMode.Acceleration);
    }
}