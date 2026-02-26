using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private InputManager inputManager;
    
    [SerializeField] private PlayerController playerController;
    
    [SerializeField] private AnimationManager animationManager;
    
    [SerializeField] private LockOnSystem lockOnSystem;
    
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private Transform shootOriginPoint;
    
    [SerializeField] private PlayerCursor playerCursor;
    
    [SerializeField] private Rigidbody rb;
    
    [SerializeField] private ProjectileBase projectilePrefab;
    
    [Tooltip("Minimum stick magnitude to update rotation (deadzone)")]
    [SerializeField] private float inputDeadzone = 0.8f;
    
    private Vector3 targetDirection = Vector3.zero;
    private Vector2 blendInput = Vector2.zero;
    private Vector3 shootDirection = Vector3.zero;
    private ShootingSystem shootingSystem = new ShootingSystem();

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        ObjectPooler.SetupPool<ProjectileBase>(projectilePrefab, 10, "Projectile");
    }

    private void OnEnable()
    {
        inputManager.OnLockToggle += lockOnSystem.ToggleLock;

        inputManager.OnAttackMelee += playerController.TryAttackMelee;
        
        playerController.OnAttackMelee  += animationManager.AttackMelee;
        
        inputManager.OnJumpPressed += playerController.TryJump;
        playerController.OnJump += HandleJump;
        
        inputManager.OnShoot += playerController.TryShoot;
        playerController.OnSimpleShoot += HandleSimpleShoot;
        playerController.OnChargeShoot += HandleChargeShoot;
    }

    private void OnDisable()
    {
        inputManager.OnLockToggle -= lockOnSystem.ToggleLock;
        
        inputManager.OnAttackMelee -= playerController.TryAttackMelee;
        
        playerController.OnAttackMelee  -= animationManager.AttackMelee;
        
        inputManager.OnJumpPressed -= playerController.TryJump;
        playerController.OnJump -= HandleJump;
        
        inputManager.OnShoot -= playerController.TryShoot;
        playerController.OnSimpleShoot -= HandleSimpleShoot;
        playerController.OnChargeShoot -= HandleChargeShoot;
    }

    private void Update()
    {
        lockOnSystem.CalculLockRotation();
        CalculateTargetDirection();
    
        playerController.SetLockMode(lockOnSystem.IsLocked);
        playerController.UpdatePlayerController(cameraTransform, inputManager.MoveInput);

        blendInput = GetBlendTreeInput(playerController.IsAttacking);
        
        animationManager.HandleAnimation(
            playerController.Rb.linearVelocity.magnitude, 
            blendInput,
            playerController.IsGrounded
        );

        shootDirection = CalculateShootDirection(inputManager.ShootInput);
        
        playerCursor.HandleRotation(shootDirection);
        
        Debug.DrawRay(shootOriginPoint.position, shootDirection * 5f, Color.red, 0.5f);
    }

    private void FixedUpdate()
    {
        playerController.UpdatePlayerControllerPhysics(targetDirection);
        lockOnSystem.HandleRotationLock(rb);
    }

    private void LateUpdate()
    {
        playerCursor.FollowPlayer();
    }

    #endregion

    #region Private Methods

    private void CalculateTargetDirection()
    {
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        targetDirection = camForward * inputManager.MoveInput.y + 
                          camRight * inputManager.MoveInput.x;
        
        if (targetDirection.magnitude > 0.1f)
        {
            targetDirection.Normalize();
        }
    }
    
    

    Vector2 GetBlendTreeInput(bool isAttacking)
    {
        if (isAttacking) return blendInput;
        
        Vector3 camR = cameraTransform.right;
        camR.y = 0f; camR.Normalize();

        Vector3 camF = cameraTransform.forward;
        camF.y = 0f; camF.Normalize();

        if (lockOnSystem.IsLocked)
        {
            Vector3 enemyDir = lockOnSystem.CurrentTarget.GetLockTransform().position
                               - transform.position;
            enemyDir.y = 0f; enemyDir.Normalize();

            float eX = Vector3.Dot(enemyDir, camR);
            float eY = Vector3.Dot(enemyDir, camF);
            
            return new Vector2(eX, eY);
        }

        if (targetDirection.magnitude < 0.1f) return Vector2.zero;

        Vector3 d = targetDirection;
        d.y = 0f; d.Normalize();

        return new Vector2(
            Vector3.Dot(d, camR),
            Vector3.Dot(d, camF)
        );
    }

    private Vector3 CalculateShootDirection(Vector2 shootInput)
    {
        if (shootInput.magnitude < inputDeadzone) return shootDirection;

        Vector3 camRight   = cameraTransform.right;
        camRight.y         = 0f;
        camRight.Normalize();

        Vector3 camForward  = cameraTransform.forward;
        camForward.y        = 0f;
        camForward.Normalize();

        Vector3 worldDirection = camRight * shootInput.x + camForward * shootInput.y;
        worldDirection.y = 0f;

        return worldDirection;
    }

    #endregion

    #region Jump

    private void HandleJump()
    {
        animationManager.Jump();
    }

    #endregion

    #region Shoot

    private void HandleSimpleShoot()
    {
        shootingSystem.SpawnProjectile(shootDirection, shootOriginPoint);
        //animationManager.EndShoot();
    }

    private void HandleChargeShoot(float chargeRatio)
    {
        shootingSystem.SpawnProjectile(shootDirection, shootOriginPoint);
        //animationManager.EndShoot();
    }

    #endregion
}