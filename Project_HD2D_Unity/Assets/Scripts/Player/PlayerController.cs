using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static readonly int CanJump = Animator.StringToHash("CanJump");

    #region Variables

    public bool IsGrounded { get; private set; }
    public Rigidbody Rb => rb;


    public event Action OnJump;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerData playerDataRaw;
    
    [SerializeField] private PlayerDataInstance playerData;
    [SerializeField] private Animator animator;
    
    private RaycastHit slopeHit;
    private bool exitingSlope;

    private bool isInLockMode = false;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        playerData = playerDataRaw.Init();
    }

    #endregion

    #region Public Methods

    public void UpdatePlayerController(Transform cam, Vector2 moveInput)
    {
        CheckGround();
        HandleRotation(cam, moveInput);
    }

    public void UpdatePlayerControllerPhysics(Vector3 targetDirection)
    {
        ApplyMovement(targetDirection);
    }

    

    #endregion

    #region Movement

    private void ApplyMovement(Vector3 targetDirection)
    {
        Vector3 targetVelocity = targetDirection * playerData.MoveSpeed;
        Vector3 currentVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 smoothedVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 0.2f);

        if (OnSlope())
        {
            smoothedVelocity = GetSlopeMoveDirection(smoothedVelocity);
        }
        
        rb.linearVelocity = new Vector3(
            smoothedVelocity.x,
            rb.linearVelocity.y,
            smoothedVelocity.z);
    }

    private void HandleRotation(Transform cam, Vector2 moveInput)
    {
        if (isInLockMode) return;

        Vector3 targetDirection = cam.forward * moveInput.y;
        targetDirection += cam.right * moveInput.x;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }
        
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            playerData.RotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
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

    public void TryJump()
    {
        if (IsGrounded && animator.GetBool(CanJump))
        {
            Jump();
        }
    }
    
    private void Jump()
    {
        
        rb.AddForce(Vector3.up * playerData.JumpForce, ForceMode.Impulse);
        
        OnJump?.Invoke();
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        float playerHeight = playerData != null
            ? playerData.PlayerHeight
            : (playerDataRaw != null ? playerDataRaw.PlayerHeight : 2f);

        float groundCheckDistance = playerData != null
            ? playerData.GroundCheckDistance
            : (playerDataRaw != null ? playerDataRaw.GroundCheckDistance : 0.2f);

        LayerMask groundMask = playerData != null
            ? playerData.GroundMask
            : (playerDataRaw != null ? playerDataRaw.GroundMask : (LayerMask)Physics.DefaultRaycastLayers);

        Vector3 rayStart = transform.position - new Vector3(0, playerHeight / 2f, 0);
        Vector3 rayEnd = rayStart - new Vector3(0, groundCheckDistance, 0);

        bool grounded = Physics.Raycast(rayStart, -Vector3.up, groundCheckDistance, groundMask);

        Gizmos.color = grounded ? Color.green : Color.red;
        Gizmos.DrawLine(rayStart, rayEnd);
        Gizmos.DrawWireSphere(rayStart, 0.1f);

        Gizmos.color = grounded ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(rayEnd, 0.15f);
    }

    #endregion

    #region Slope And Stairs

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
        return Vector3.ProjectOnPlane(direction, slopeHit.normal);
    }

    #endregion

    #region Lock

    public void SetLockMode(bool locked)
    {
        isInLockMode = locked;
    }

    #endregion
}