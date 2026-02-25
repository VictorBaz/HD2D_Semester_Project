using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables
    
    public bool IsGrounded { get; private set; }
    public Rigidbody Rb => rb;
    public bool IsAttacking { get;private set; }

    public event Action OnJump;
    public event Action OnAttackMelee;
    public event Action OnSimpleShoot;
    public event Action<float> OnChargeShoot;
    public event Action OnStartChargingShoot;
    public event Action<float> OnChargeTick;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerData playerDataRaw;
    
    [SerializeField] private PlayerDataInstance playerData;
    [SerializeField] private Animator animator;
    
    [SerializeField] private float chargeThreshold = 0.2f;
    [SerializeField] private float maxChargeTime = 2f;
    
    [SerializeField] private float dashSpeed = 6f;
    [SerializeField] private float dashDuration = 6f;
    
    
    private RaycastHit slopeHit;
    private bool exitingSlope;
    
    private bool isInLockMode = false;
    
    private float shootPressTime;
    private bool isChargingShoot;
    private Quaternion targetRotation;


    private static readonly int CanJump = Animator.StringToHash("CanJump");
    private static readonly int Attacking = Animator.StringToHash("IsAttacking");
    
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
        HandleChargeTick();
    }

    public void UpdatePlayerControllerPhysics(Vector3 targetDirection)
    {
        ApplyMovement(targetDirection);
        
        if (targetRotation != Quaternion.identity)
        {
            targetRotation.Normalize();
            rb.MoveRotation(targetRotation);
        }
    }
    
    #endregion

    #region Movement

    private void ApplyMovement(Vector3 targetDirection)
    {
        if(IsAttacking) return;
        
        Vector3 targetVelocity = targetDirection * playerData.MoveSpeed;
        Vector3 currentVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 smoothedVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 0.2f);

        if (OnSlope())
        {
            smoothedVelocity = GetSlopeMoveDirection(smoothedVelocity);
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

    private void HandleRotation(Transform cam, Vector2 moveInput)
    {
        if (isInLockMode && IsAttacking) return;

        Vector3 targetDirection = cam.forward * moveInput.y;
        targetDirection += cam.right * moveInput.x;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }
        
        targetRotation = Quaternion.LookRotation(targetDirection);
        targetRotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(targetDirection),
            playerData.RotationSpeed * Time.fixedDeltaTime);

        //transform.rotation = playerRotation;
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
        if (IsGrounded && !IsAttacking && !IsLanding() && !IsInAir())
        {
            Jump();
        }
    }
    
    private void Jump()
    {
        rb.AddForce(Vector3.up * playerData.JumpForce, ForceMode.Impulse);
        
        OnJump?.Invoke();
    }

    private bool IsLanding()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Land");
    }

    private bool IsInAir()
    {
        return  animator.GetCurrentAnimatorStateInfo(0).IsName("Fall");
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

    #region Attack

    public void TryAttackMelee()
    {
        if (IsAttacking || !IsGrounded) return;
        
        AttackMelee();
    }

    private void AttackMelee()
    {
        OnAttackMelee?.Invoke();
        StartCoroutine(AttackMeleeIe());
    }

    private IEnumerator AttackMeleeIe()
    {
        IsAttacking = true;

        dashDuration = 0.35f;
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            float t = elapsed / dashDuration;
            
            rb.linearVelocity = Vector3.Lerp(
                transform.forward * dashSpeed,
                Vector3.zero,
                t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        ToggleFixPlayerPosition(true);
        yield return new WaitForSeconds(
            playerData.GetLengthOfClip(playerData.AttackClip) - dashDuration);

        ToggleFixPlayerPosition(false);
        IsAttacking = false;
    }

    public void TryShoot(InputAction.CallbackContext ctx)
    {
        if (IsAttacking || !IsGrounded)
        {
            return;
        }

        if (ctx.started)
        {
            isChargingShoot = true;
            shootPressTime = Time.time;
            OnStartChargingShoot?.Invoke();
        }
        else if (ctx.canceled && isChargingShoot)
        {
            isChargingShoot = false;

            float holdDuration = Time.time - shootPressTime;

            if (holdDuration < chargeThreshold)
            {
                Debug.Log("Simple shoot fired");
                OnSimpleShoot?.Invoke();
            }
            else
            {
                float chargeRatio = Mathf.Clamp01(holdDuration / maxChargeTime);
                Debug.Log($"Charged shoot fired — charge ratio: {chargeRatio:F2}");
                OnChargeShoot?.Invoke(chargeRatio);
            }
        }
    }

    private void HandleChargeTick()
    {
        if (!isChargingShoot) return;

        float chargeRatio = Mathf.Clamp01((Time.time - shootPressTime) / maxChargeTime);
        OnChargeTick?.Invoke(chargeRatio);
    }
    #endregion

    #region Contraints

    private void ToggleFixPlayerPosition(bool fixedPosition)
    {
        if (fixedPosition)
        {
            rb.linearVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezePosition;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
    

    #endregion

    

}