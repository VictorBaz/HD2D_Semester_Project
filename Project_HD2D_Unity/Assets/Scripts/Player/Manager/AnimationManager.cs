using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    #region Variables
    public Camera cam;
    [SerializeField] private Transform mainTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private static readonly int MoveXHash = Animator.StringToHash("moveX");
    private static readonly int MoveYHash = Animator.StringToHash("moveY");
    
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    
    private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    
    private static readonly int ComboIndexHash = Animator.StringToHash("ComboIndex");
    
    private static readonly int InputMagnitudeHash = Animator.StringToHash("InputMagnitude");
    
    private static readonly int DashingHash = Animator.StringToHash("Dashing");
    
    private static readonly int IsCarrying = Animator.StringToHash("IsCarrying");

    #endregion

    #region Public Methods

    public void HandleAnimation(float inputRawMagnitude, Vector2 inputBlendTree, bool isGrounded)
    {
        UpdateMovement(inputBlendTree);
        GroundedParameters(isGrounded);
        UpdateInputMagnitude(inputRawMagnitude);
    }
    
    public bool IsLandingFinished()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return !info.IsName("Land");
    }

    public bool IsInAttackAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }

    private void UpdateMovement(Vector2 input)
    {
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

    public void UpdateInputMagnitude(float magnitude)
    {
        animator.SetFloat(InputMagnitudeHash, magnitude);
    }

    public void Jump()
    {
        animator.SetTrigger(JumpHash);
    }

    private void GroundedParameters(bool isGrounded)
    {
        animator.SetBool(IsGroundedHash, isGrounded);
    }
    
    public void ExitAttack()
    {
        animator.SetBool(IsAttackingHash, false);
        animator.SetInteger(ComboIndexHash, 0);
    }
    
    public void SetComboIndex(int index)
    {
        animator.SetInteger(ComboIndexHash, index);
        animator.SetBool(IsAttackingHash, true);
    }

    public void SetDash(bool isDashing)
    {
        animator.SetBool(DashingHash, isDashing);
    }

    public void SetIsCarrying(bool isCarrying)
    {
        animator.SetBool(IsCarrying, isCarrying);
    }

    public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
    {
        return animator.GetCurrentAnimatorStateInfo(layerIndex);
    }
    
    #endregion
}