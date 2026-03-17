using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    public bool IsGrounded  { get; private set; }
    public bool IsAttacking { get; private set; }
    public Rigidbody Rb => rb;

    public event Action OnJump;
    public Action OnAttackMelee;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;

    private RaycastHit slopeHit;
    private bool isInLockMode;
    private Quaternion targetRotation;

    private static readonly int CanJump = Animator.StringToHash("CanJump");
    private static readonly int Attacking = Animator.StringToHash("IsAttacking");

    private PlayerDataInstance playerData;
    private float currentSpeed = 0f;
    
    private bool isJumping;
    
    #endregion


    #region Public Methods

    public void UpdatePlayerController(Transform cam, Vector2 moveInput)
    {
        CheckGround();
        HandleRotation(cam, moveInput);
    }

    public void UpdatePlayerControllerPhysics(Vector3 targetDirection, Vector2 moveInput, float speedMultiplier)
    {
        ApplyMovement(targetDirection, moveInput, speedMultiplier);

        if (targetRotation != Quaternion.identity)
        {
            targetRotation.Normalize();
            rb.MoveRotation(targetRotation);
        }
    }

    #endregion

    #region Movement

    private void ApplyMovement(Vector3 targetDirection, Vector2 moveInput, float speedMultiplier)
    {
        bool onSlope = OnSlope();
        bool isMoving = moveInput.magnitude > 0.01f;

        if (isJumping || !IsGrounded) 
        {
            rb.useGravity = true; 
            Vector3 targetVel = targetDirection * currentSpeed;
        
            rb.linearVelocity = new Vector3(targetVel.x, rb.linearVelocity.y, targetVel.z);
            return; 
        }
 
        rb.useGravity = !onSlope;
        
        float targetSpeed = (onSlope ? playerData.MoveSpeedSlope : 
                                (moveInput.magnitude >= playerData.RunThreshold ? playerData.MoveSpeedRunning : playerData.MoveSpeedWalking)) 
                            * speedMultiplier;

        float accel = isMoving ? playerData.Acceleration : playerData.Deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);

        Vector3 targetVelocity = targetDirection * currentSpeed;

        if (onSlope)
        {
            Vector3 slopeDir = GetSlopeMoveDirection(targetVelocity);
        
            if (!isMoving && rb.linearVelocity.magnitude < 0.1f)
            {
                rb.linearVelocity = Vector3.zero;
            }
            else
            {
                rb.linearVelocity = slopeDir;
            
                rb.AddForce(-slopeHit.normal * 30f, ForceMode.Acceleration);
            }
        }
        else
        {
            float yVel = rb.linearVelocity.y;
            
            if (IsGrounded && yVel > 0) yVel = 0; 

            rb.linearVelocity = new Vector3(targetVelocity.x, yVel, targetVelocity.z);
        }
    }

    private float SelectSpeed(Vector2 moveInput)
    {
        if (OnSlope()) return playerData.MoveSpeedSlope;

        return moveInput.magnitude >= playerData.RunThreshold
            ? playerData.MoveSpeedRunning
            : playerData.MoveSpeedWalking;
    }
    

    private void HandleRotation(Transform cam, Vector2 moveInput)
    {
        if (isInLockMode && IsAttacking) return;

        Vector3 targetDirection = cam.forward * moveInput.y + cam.right * moveInput.x;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        targetRotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(targetDirection),
            playerData.RotationSpeed * Time.fixedDeltaTime);
    }

    #endregion

    #region Ground Check

    private void CheckGround()
    {
        float sphereRadius = 0.2f;
        Vector3 rayStart = transform.position - new Vector3(0, (playerData.PlayerHeight / 2) - sphereRadius, 0);
        
        IsGrounded = Physics.SphereCast(
            rayStart,
            sphereRadius,
            -Vector3.up,
            out _,
            playerData.GroundCheckDistance,
            playerData.GroundMask);
    }

    #endregion

    #region Jump
    
    public void SetJumping(bool jumping) => isJumping = jumping;
    
    public void Jump()
    {
        rb.AddForce(Vector3.up * playerData.JumpForce, ForceMode.Impulse);
        OnJump?.Invoke();
    }

    private bool IsLanding() => animator.GetCurrentAnimatorStateInfo(0).IsName("Land");
    private bool IsInAir()   => animator.GetCurrentAnimatorStateInfo(0).IsName("Fall");

    #endregion

    #region Lock

    public void SetLockMode(bool locked)
    {
        isInLockMode = locked;
    }

    #endregion

    #region Attack
    
    public Coroutine RunRoutine(IEnumerator routine) => StartCoroutine(routine);

    #endregion

    #region Constraints

    public void ToggleFixPlayerPosition(bool fixedPosition)
    {
        if (fixedPosition)
        {
            rb.linearVelocity = Vector3.zero;
            rb.constraints    = RigidbodyConstraints.FreezePosition;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    #endregion

    #region Slope And Stairs

    
    
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit,
                playerData.PlayerHeight * 0.5f + 0.2f, playerData.GroundMask))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < playerData.MaxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized * direction.magnitude;
    }

    #endregion

    public void InitData(PlayerDataInstance data)
    {
        playerData = data;
    }
   
}