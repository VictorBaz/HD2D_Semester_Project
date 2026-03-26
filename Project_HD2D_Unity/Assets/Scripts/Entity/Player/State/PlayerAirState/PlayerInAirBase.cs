using UnityEngine;

public abstract class PlayerInAirBase : PlayerBaseState
{
    public override string Name => "InAir";
    public override bool CanDash => true;

    protected void HandleAirMovement(PlayerStateContext psc)
    {   
        /*float airControl = Mathf.Lerp(1f, 0.4f, Mathf.Abs(psc.Rb.linearVelocity.y) / 10f);
        HandlePhysics(psc, airControl);*/
        HandleMovement(psc);
        HandleAnimation(psc);
    }

    protected void AirControl(PlayerStateContext psc)
    {
        float airControl = (Name == "Jump") ? 0.2f : (Name == "Fall") ? 0.1f : 0f;
        
        HandlePhysics(psc);
    }
}