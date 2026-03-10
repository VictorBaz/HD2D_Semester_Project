using UnityEngine;

public abstract class PlayerBaseState
{
    protected  Vector3 targetDirection = Vector3.zero;
    protected Vector2 blendInput = Vector2.zero;
    protected Vector3 shootDirection = Vector3.zero;
    
    public abstract void EnterState(PlayerStateContext psc);
    public abstract void ExitState(PlayerStateContext psc);
    public abstract void UpdateState(PlayerStateContext psc);
    public abstract void FixedUpdateState(PlayerStateContext psc);
    
    public virtual bool CanJump => false;
    public virtual bool CanAttack => false;
    public virtual bool CanMove => true;
    public virtual bool CanShoot => true;
    
    public virtual string Name { get; protected set; }
    
    protected void HandlePhysics(PlayerStateContext psc, float speedMultiplier = 1f)
    {
        if (!CanMove) return;
        psc.Controller.UpdatePlayerControllerPhysics(targetDirection, speedMultiplier);
    }
    
    protected virtual void CalculateTargetDirection(PlayerStateContext psc)
    {
        Vector3 camForward = psc.CameraTransform.forward;
        camForward.y = 0; camForward.Normalize();

        Vector3 camRight = psc.CameraTransform.right;
        camRight.y = 0; camRight.Normalize();

        targetDirection = camForward * psc.InputManager.MoveInput.y +
                          camRight   * psc.InputManager.MoveInput.x;

        if (targetDirection.magnitude > 0.1f)
            targetDirection.Normalize();
    }
    
    protected virtual Vector2 GetBlendTreeInput(PlayerStateContext psc)
    {
        Vector3 camR = psc.CameraTransform.right;
        camR.y = 0f; camR.Normalize();

        Vector3 camF = psc.CameraTransform.forward;
        camF.y = 0f; camF.Normalize();

        if (psc.LockOnSystem.IsLocked)
        {
            Vector3 enemyDir = psc.LockOnSystem.CurrentTarget.GetLockTransform().position
                               - psc.PlayerTransform.position;
            enemyDir.y = 0f; enemyDir.Normalize();

            return new Vector2(
                Vector3.Dot(enemyDir, camR),
                Vector3.Dot(enemyDir, camF));
        }

        if (targetDirection.magnitude < 0.1f) return Vector2.zero;

        Vector3 d = targetDirection;
        d.y = 0f; d.Normalize();

        return new Vector2(
            Vector3.Dot(d, camR),
            Vector3.Dot(d, camF));
    }
    
    protected Vector3 CalculateShootDirection(PlayerStateContext psc)
    {
        if (psc.InputManager.ShootInput.magnitude < psc.PlayerData.InputDeadzone) return shootDirection;

        Vector3 camRight = psc.CameraTransform.right;
        camRight.y = 0f; camRight.Normalize();

        Vector3 camForward = psc.CameraTransform.forward;
        camForward.y = 0f; camForward.Normalize();

        Vector3 worldDirection = camRight * psc.InputManager.ShootInput.x 
        + camForward * psc.InputManager.ShootInput.y;
        worldDirection.y = 0f;

        return worldDirection;
    }
    
    protected void HandleMovement(PlayerStateContext psc)
    {
        if (!CanMove) return;
        
        CalculateTargetDirection(psc);
        psc.Controller.UpdatePlayerController(psc.CameraTransform, psc.InputManager.MoveInput);
    }
    
    protected void HandleCursor(PlayerStateContext psc)
    {
        shootDirection = CalculateShootDirection(psc); 
        psc.PlayerCursor.HandleRotation(shootDirection);
        psc.ShootingSystem.SetShootDirection(shootDirection);
    }
    
    protected void HandleAnimation(PlayerStateContext psc)
    {
        blendInput = GetBlendTreeInput(psc);
        psc.AnimationManager.HandleAnimation(
            psc.Rb.linearVelocity.magnitude,
            blendInput,
            psc.Controller.IsGrounded);
    }
}
