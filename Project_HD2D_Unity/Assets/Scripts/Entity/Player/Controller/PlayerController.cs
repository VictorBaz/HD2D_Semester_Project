using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    public bool IsGrounded { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsOnWall { get; private set; }
    public Rigidbody Rb => rb;

    public Action OnJump;
    public Action OnAttackMelee;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private LockOnSystem lockOnSystem;
    [SerializeField] private Camera cam;

    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private Vector3 attackColliderLocalOffset = new Vector3(0f, 0.5f, 0.8f);
    [SerializeField] private float wallCheckDistance = 0.6f;

    private static readonly Collider[] _hitBuffer = new Collider[10];
    public Collider[] HitBuffer => _hitBuffer;

    private static readonly Vector3[] _wallDirections =
    {
        Vector3.forward, Vector3.back, Vector3.right, Vector3.left
    };

    private PlayerDataInstance playerData;
    private RaycastHit slopeHit;
    private Quaternion targetRotation;
    private Vector3 wallNormal;
    private float currentSpeed;
    private bool isJumping;
    private bool isInLockMode;
    
    public bool Blocked { get;  set; }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (rb) rb.useGravity = false;
    }

    private void Start() => targetRotation = transform.rotation;


    #endregion

    #region Public Methods

    public void UpdatePlayerController(Transform cam, Vector2 moveInput)
    {
        if (Blocked) return;
        
        CheckGround();
        CheckWall();
        HandleRotation(cam, moveInput);
    }

    public void UpdatePlayerControllerPhysics(Vector3 targetDirection, Vector2 moveInput, float speedMultiplier)
    {
        
        if (Blocked)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        
        ApplyMovement(targetDirection, moveInput, speedMultiplier);

        if (targetRotation != Quaternion.identity)
        {
            targetRotation.Normalize();
            rb.MoveRotation(targetRotation);
        }
    }

    public void SetJumping(bool jumping) => isJumping = jumping;
    public void InitData(PlayerDataInstance data) => playerData = data;
    public void SetGravity(bool useGravity) => rb.useGravity = useGravity;
    public Coroutine RunRoutine(IEnumerator routine) => StartCoroutine(routine);
    public void SetLockMode(bool locked) => isInLockMode = locked;

    public void Jump()
    {
        isJumping = true;
        OnJump?.Invoke();
    }

    public int OverlapAttack(LayerMask layer)
    {
        Vector3 pos = transform.TransformPoint(attackColliderLocalOffset);
        return Physics.OverlapSphereNonAlloc(pos, attackRadius, _hitBuffer, layer);
    }

    #endregion

    #region Movement

    private void ApplyMovement(Vector3 targetDirection, Vector2 moveInput, float speedMultiplier)
    {
        float targetSpeed = CalculateTargetSpeed(moveInput, speedMultiplier);
        UpdateCurrentSpeed(targetSpeed, moveInput.sqrMagnitude > GameConstants.DEAD_STICK_SQUARE);

        if (IsGrounded && !isJumping)
            ApplyGroundMovement(targetDirection);
        else
            ApplyAirMovement(moveInput);
    }

    private float CalculateTargetSpeed(Vector2 moveInput, float speedMultiplier)
    {
        if (moveInput.sqrMagnitude <= GameConstants.DEAD_STICK_SQUARE) return 0f;

        float speed = OnSlope()                                    ? playerData.MoveSpeedSlope   :
                      moveInput.magnitude >= playerData.RunThreshold ? playerData.MoveSpeedRunning :
                                                                       playerData.MoveSpeedWalking;
        return speed * speedMultiplier;
    }

    private void UpdateCurrentSpeed(float targetSpeed, bool isMoving)
    {
        float accel = isMoving ? playerData.Acceleration : playerData.Deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);

        if (!isMoving && currentSpeed < GameConstants.VELOCITY_TO_SNAP_TO_0)
            currentSpeed = 0f;
    }

    private void ApplyGroundMovement(Vector3 targetDirection)
    {
        Vector3 targetVelocity = targetDirection * currentSpeed;

        if (OnSlope())
        {
            rb.linearVelocity = GetSlopeMoveDirection(targetVelocity);
        }
        else
        {
            float yVel = isJumping ? rb.linearVelocity.y : 0f;
            rb.linearVelocity = new Vector3(targetVelocity.x, yVel, targetVelocity.z);
        }
    }

    private void ApplyAirMovement(Vector2 moveInput)
    {
        Vector3 camForward = cam.transform.forward; camForward.y = 0f;
        Vector3 camRight   = cam.transform.right;   camRight.y   = 0f;

        Vector3 targetDir          = (camForward * moveInput.y + camRight * moveInput.x).normalized;
        Vector3 currentHorizontal  = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 newHorizontal      = currentHorizontal + targetDir * (playerData.AirControlForce * Time.fixedDeltaTime);

        if (newHorizontal.magnitude > playerData.MoveSpeedRunning)
            newHorizontal = Vector3.ClampMagnitude(newHorizontal, Mathf.Max(currentHorizontal.magnitude, playerData.MoveSpeedRunning));

        rb.linearVelocity = new Vector3(newHorizontal.x, rb.linearVelocity.y, newHorizontal.z);

        if (IsOnWall) CancelWallVelocity();
    }

    private void CancelWallVelocity()
    {
        Vector3 horizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (Vector3.Dot(horizontal.normalized, wallNormal) >= 0f) return;

        Vector3 projected = Vector3.ProjectOnPlane(horizontal, wallNormal);
        rb.linearVelocity = new Vector3(projected.x, rb.linearVelocity.y, projected.z);
    }

    #endregion

    #region Ground, Slopes & Walls

    private void CheckGround()
    {
        Vector3 rayStart = transform.position - new Vector3(0f, (playerData.PlayerHeight / 2f) - GameConstants.CHECK_GROUND_RADIUS, 0f);
        IsGrounded = Physics.SphereCast(rayStart, GameConstants.CHECK_GROUND_RADIUS, Vector3.down, out _, playerData.GroundCheckDistance, playerData.GroundMask,QueryTriggerInteraction.Ignore);
    }

    private void CheckWall()
    {
        if (IsGrounded) { IsOnWall = false; return; }

        foreach (Vector3 localDir in _wallDirections)
        {
            Vector3 worldDir = transform.TransformDirection(localDir);

            if (!Physics.Raycast(transform.position, worldDir, out RaycastHit hit, wallCheckDistance, playerData.GroundMask,QueryTriggerInteraction.Ignore))
                continue;

            if (Mathf.Abs(hit.normal.y) < 0.3f)
            {
                IsOnWall   = true;
                wallNormal = hit.normal;
                return;
            }
        }

        IsOnWall   = false;
        wallNormal = Vector3.zero;
    }
    
    public bool IsFacingWall()
    {
        foreach (Vector3 localDir in _wallDirections)
        {
            Vector3 worldDir = transform.TransformDirection(localDir);

            if (!Physics.Raycast(transform.position, worldDir, wallCheckDistance, playerData.GroundMask,QueryTriggerInteraction.Ignore))
                continue;

            return true;
        }

        return false;
    }

    public bool OnSlope()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerData.PlayerHeight * 0.5f + 0.2f, playerData.GroundMask,QueryTriggerInteraction.Ignore))
            return false;

        float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
        return angle < playerData.MaxSlopeAngle && angle != 0f;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized * direction.magnitude;
    }

    #endregion

    #region Rotation

    private void HandleRotation(Transform cam, Vector2 moveInput)
    {
        if (lockOnSystem.IsLocked)
        {
            Vector3 lookDir = (lockOnSystem.CurrentTarget.GetLockTransform().position - transform.position).normalized;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > GameConstants.DEAD_STICK_SQUARE)
                targetRotation = Quaternion.LookRotation(lookDir);
            return;
        }

        if (moveInput.sqrMagnitude <= GameConstants.SNAP_BACK) return;

        Vector3 camForward = cam.forward; camForward.y = 0f;
        Vector3 camRight   = cam.right;   camRight.y   = 0f;
        Vector3 moveDir    = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        if (moveDir.sqrMagnitude > 0.001f)
            targetRotation = Quaternion.LookRotation(moveDir);
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        Vector3 attackPos = transform.TransformPoint(attackColliderLocalOffset);

        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.4f);
        Gizmos.DrawSphere(attackPos, attackRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRadius);

        if (!Application.isPlaying || !IsOnWall) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, wallNormal);
    }

    #endregion
}