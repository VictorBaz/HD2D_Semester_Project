using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    public bool IsGrounded  { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsJumping => isJumping;
    public Rigidbody Rb => rb;

    public event Action OnJump;
    public Action OnAttackMelee;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private LockOnSystem lockOnSystem;

    private RaycastHit slopeHit;
    private bool isInLockMode;
    private Quaternion targetRotation;

    private PlayerDataInstance playerData;
    private float currentSpeed = 0f;
    
    private bool isJumping;
    #endregion

    private void Awake()
    {
        if (rb != null) rb.useGravity = false; 
    }

    #region Public Methods
    public void UpdatePlayerController(Transform cam, Vector2 moveInput)
    {
        CheckGround();
        HandleRotation(cam, moveInput);

        rb.useGravity = !IsGrounded || isJumping;
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

    public void SetJumping(bool jumping)
    {
        isJumping = jumping;
    }
    #endregion

    #region Movement
    private void ApplyMovement(Vector3 targetDirection, Vector2 moveInput, float speedMultiplier)
    {
        bool isMoving = moveInput.magnitude > 0.01f;
        bool onSlope = OnSlope();

        float targetSpeed = 0f;
        if (isMoving)
        {
            if (onSlope) targetSpeed = playerData.MoveSpeedSlope;
            else if (moveInput.magnitude >= playerData.RunThreshold) targetSpeed = playerData.MoveSpeedRunning;
            else targetSpeed = playerData.MoveSpeedWalking;
        }
        targetSpeed *= speedMultiplier;

        float accel = isMoving ? playerData.Acceleration : playerData.Deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);

        Vector3 targetVelocity = targetDirection * currentSpeed;

        if (IsGrounded && !isJumping)
        {
            if (onSlope)
            {
                rb.linearVelocity = GetSlopeMoveDirection(targetVelocity);
            }
            else
            {
                rb.linearVelocity = new Vector3(targetVelocity.x, 0f, targetVelocity.z);
            }

            if (!isMoving && currentSpeed < 0.1f)
            {
                rb.linearVelocity = Vector3.zero;
            }
        }
        else
        {
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        }
    }
    #endregion

    #region Ground & Slopes (Ton code original conservé)
    private void CheckGround()
    {
        float sphereRadius = 0.2f;
        Vector3 rayStart = transform.position - new Vector3(0, (playerData.PlayerHeight / 2) - sphereRadius, 0);
        IsGrounded = Physics.SphereCast(rayStart, sphereRadius, -Vector3.up, out _, playerData.GroundCheckDistance, playerData.GroundMask);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerData.PlayerHeight * 0.5f + 0.2f, playerData.GroundMask))
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

    #region Rotation & Utility
    private void HandleRotation(Transform cam, Vector2 moveInput)
    {
        if (lockOnSystem.IsLocked) 
        {
            Vector3 targetPos = lockOnSystem.CurrentTarget.GetLockTransform().position;
            Vector3 lookDir = (targetPos - transform.position).normalized;
            lookDir.y = 0;
            if (lookDir != Vector3.zero) targetRotation = Quaternion.LookRotation(lookDir);
            return;
        }

        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0; camRight.y = 0;
            Vector3 moveDir = (camForward * moveInput.y + camRight * moveInput.x).normalized;
            if (moveDir != Vector3.zero) targetRotation = Quaternion.LookRotation(moveDir);
        }
    }

    public void Jump()
    {
        isJumping = true;
        rb.useGravity = true;
        rb.AddForce(Vector3.up * playerData.JumpForce, ForceMode.Impulse);
        OnJump?.Invoke();
    }

    public void InitData(PlayerDataInstance data) => playerData = data;
    public void SetLockMode(bool locked) => isInLockMode = locked;
    public Coroutine RunRoutine(IEnumerator routine) => StartCoroutine(routine);
    
    #endregion
}