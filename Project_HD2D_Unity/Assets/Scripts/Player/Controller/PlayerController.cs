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
        float targetSpeed  = SelectSpeed(moveInput) * speedMultiplier;

        float acceleration = moveInput.magnitude > 0.1f
            ? playerData.Acceleration
            : playerData.Deceleration;

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        Vector3 targetVelocity   = targetDirection * currentSpeed;
        Vector3 currentVelocity  = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 smoothedVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 0.2f);

        if (OnSlope())
        {
            smoothedVelocity  = GetSlopeMoveDirection(smoothedVelocity);
            rb.linearVelocity = smoothedVelocity;
        }
        else
        {
            rb.linearVelocity = new Vector3(
                smoothedVelocity.x,
                rb.linearVelocity.y,
                smoothedVelocity.z);
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
        Vector3 rayStart = transform.position - new Vector3(0, playerData.PlayerHeight / 2, 0);
        IsGrounded = Physics.Raycast(
            rayStart,
            -Vector3.up,
            playerData.GroundCheckDistance,
            playerData.GroundMask);
    }

    #endregion

    #region Jump
    

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
        return Vector3.ProjectOnPlane(direction, slopeHit.normal);
    }

    #endregion

    public void InitData(PlayerDataInstance data)
    {
        playerData = data;
    }
   
}