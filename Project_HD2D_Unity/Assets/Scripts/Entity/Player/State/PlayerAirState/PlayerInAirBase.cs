using UnityEngine;

public abstract class PlayerInAirBase : PlayerBaseState
{
    public override string Name   => "InAir";
    public override bool CanDash  => true;
    
    private RaycastHit pendingSnapHit;
    private bool       hasPendingSnap;

    protected void HandleAirMovement(PlayerStateContext psc)
    {
        HandleMovement(psc);
        HandleAnimation(psc);
        CheckSnapToPlatformEdge(psc);
    }

    protected void AirControl(PlayerStateContext psc)
    {
        HandlePhysics(psc);
        ApplySnapToPlatformEdge(psc);
    }


    private void CheckSnapToPlatformEdge(PlayerStateContext psc)
    {
        CapsuleCollider col = psc.Collider;
        
        float radius = col.radius;

        float offsetToBottomSphere = (col.height / 2f) - radius;

        Vector3 snapOrigin = psc.Rb.position 
                             + Vector3.down * offsetToBottomSphere 
                             + psc.Controller.transform.forward * (radius * 0.8f);

        snapOrigin += Vector3.up * 0.1f;

        float rayDistance = radius; 

        Debug.DrawRay(snapOrigin, Vector3.down * rayDistance, Color.cyan);

        hasPendingSnap = Physics.Raycast(snapOrigin, Vector3.down, out pendingSnapHit, rayDistance, psc.PlayerData.GroundMask)
                         && Vector3.Dot(pendingSnapHit.normal, Vector3.up) >= 0.9f;
    }

    private void ApplySnapToPlatformEdge(PlayerStateContext psc)
    {
        if (!hasPendingSnap) return;

        CapsuleCollider col = psc.Collider;
        psc.Rb.MovePosition(pendingSnapHit.point + Vector3.up * (col.height / 2f));
        psc.Rb.linearVelocity = new Vector3(psc.Rb.linearVelocity.x, 0f, psc.Rb.linearVelocity.z);
        hasPendingSnap = false;
    }
}