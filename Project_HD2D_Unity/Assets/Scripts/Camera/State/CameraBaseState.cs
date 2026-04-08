using UnityEngine;

public abstract class CameraBaseState
{
    public abstract void EnterState(CameraStateContext context);
    public abstract void UpdateState(CameraStateContext context);
    public abstract void ExitState(CameraStateContext context);
    
    protected Vector3 CalculateCollision(Vector3 targetPos, Vector3 playerPos,float collisionPadding,LayerMask collisionLayers, float thickness = 0.2f)
    {
        Vector3 direction = targetPos - playerPos;
        float distance = direction.magnitude;

        if (Physics.SphereCast(playerPos, thickness, direction.normalized, out RaycastHit hit, distance, collisionLayers))
        {
            return playerPos + direction.normalized * (hit.distance - collisionPadding);
        }

        return targetPos;
    }
}