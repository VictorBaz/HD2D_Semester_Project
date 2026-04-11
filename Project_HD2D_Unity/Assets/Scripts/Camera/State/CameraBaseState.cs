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

    protected void ApplyRestrictedRotation(CameraStateContext context, Vector3 targetPos)
    {
        Vector3 direction = targetPos - context.CameraTransform.position;
    
        if (direction.sqrMagnitude < 0.01f) return; 

        Quaternion fullRotation = Quaternion.LookRotation(direction);
    
        Vector3 currentEuler = fullRotation.eulerAngles;
        Quaternion targetRotation = Quaternion.Euler(
            context.Manager.FixedX, 
            currentEuler.y, 
            context.Manager.FixedZ
        );

        context.CameraTransform.rotation = Quaternion.Slerp(
            context.CameraTransform.rotation, 
            targetRotation, 
            Time.deltaTime / context.Manager.RotationSmoothTime
        );
    }
}