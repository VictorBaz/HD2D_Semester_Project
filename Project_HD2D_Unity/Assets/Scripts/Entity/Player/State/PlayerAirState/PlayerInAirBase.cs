using UnityEngine;

public abstract class PlayerInAirBase : PlayerBaseState
{
    public override string Name => "InAir";
    public override bool CanDash => true;

    protected void HandleAirMovement(PlayerStateContext psc)
    {   
        HandleMovement(psc);
        HandleAnimation(psc);

        CapsuleCollider collider = psc.StateMachine.GetComponentInChildren<CapsuleCollider>();
        Vector3 snapOrigin = psc.Controller.transform.position +
                             psc.Controller.transform.forward * collider.radius +
                             Vector3.down * (collider.height / 2f - collider.radius);
        Ray platformSnapRay = new Ray(snapOrigin, Vector3.down);
        Debug.DrawRay(platformSnapRay.origin, platformSnapRay.direction * collider.radius, Color.cyan);
        if (Physics.Raycast(platformSnapRay, out RaycastHit hit, collider.radius))
        {
            if (hit.normal == Vector3.up)
            {
                psc.StateMachine.transform.position = hit.point + Vector3.up * collider.height / 2f;
            }
        }
    }

    protected void AirControl(PlayerStateContext psc)
    {
        HandlePhysics(psc);
    }
}