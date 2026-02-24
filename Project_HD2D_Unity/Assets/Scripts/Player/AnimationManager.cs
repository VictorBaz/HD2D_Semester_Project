using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    #region Variables
    public Camera cam;
    [SerializeField] private Transform mainTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private static readonly int VelocityHash = Animator.StringToHash("Velocity");
    private static readonly int MoveXHash = Animator.StringToHash("moveX");
    private static readonly int MoveYHash = Animator.StringToHash("moveY");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int MeleeAttack = Animator.StringToHash("MeleeAttack");
    private static readonly int IsChargingHash = Animator.StringToHash("IsCharging");

    #endregion

    #region Public Methods

    public void HandleAnimation(float velocity, Vector2 input, bool isGrounded)
    {
        UpdateMovement(velocity, input);
        
        GroundedParameters(isGrounded);
    }

    private void UpdateMovement(float velocity, Vector2 input)
    {
        animator.SetFloat(VelocityHash, velocity);

        if (input.magnitude > 0.1f)
        {
            animator.SetFloat(
                MoveXHash, 
                Mathf.Lerp(animator.GetFloat(MoveXHash), input.x, 15f * Time.deltaTime));
            animator.SetFloat(
                MoveYHash, 
                Mathf.Lerp(animator.GetFloat(MoveYHash), input.y, 15f * Time.deltaTime));
        }
    }

    public void Jump()
    {
        animator.SetTrigger(JumpHash);
    }

    private void GroundedParameters(bool isGrounded)
    {
        animator.SetBool(IsGroundedHash, isGrounded);
    }

    public void AttackMelee()
    {
        animator.SetTrigger(MeleeAttack);
    }
    
    
    #endregion
}