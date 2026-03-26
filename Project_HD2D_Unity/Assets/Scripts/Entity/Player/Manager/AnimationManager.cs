using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    #region Animator Hashes
    private static readonly int MoveXHash = Animator.StringToHash("moveX");
    private static readonly int MoveYHash = Animator.StringToHash("moveY");
    private static readonly int InputMagnitudeHash = Animator.StringToHash("InputMagnitude");
    
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int IsFallingHash = Animator.StringToHash("IsFalling");
    private static readonly int DashingHash = Animator.StringToHash("Dashing");
    private static readonly int IsCarryingHash = Animator.StringToHash("IsCarrying");
    private static readonly int IsHitHash = Animator.StringToHash("IsHit");
    
    private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int ComboIndexHash = Animator.StringToHash("ComboIndex");
    
    private static readonly int JumpTriggerHash = Animator.StringToHash("Jump");
    private static readonly int VelocityYHash = Animator.StringToHash("VelocityY");
    
    private static readonly int IsParryingHash = Animator.StringToHash("IsParrying");
    #endregion

    [SerializeField] private Animator animator;
    
    [Header("Settings")]
    [SerializeField] private float dampTime = 0.1f; 

    #region Core Update logic
    
    public void HandleAnimation(float inputRawMagnitude, Vector2 inputBlendTree, bool isGrounded, Vector3 velocity)
    {
        UpdateMovement(inputBlendTree);
        
        animator.SetFloat(InputMagnitudeHash, inputRawMagnitude);
        
        animator.SetBool(IsGroundedHash, isGrounded);
        
        animator.SetFloat(VelocityYHash, velocity.y);
        
        bool isFalling = !isGrounded && velocity.y < -0.1f;
        animator.SetBool(IsFallingHash, isFalling);
    }

    private void UpdateMovement(Vector2 input)
    {
        animator.SetFloat(MoveXHash, input.x, dampTime, Time.deltaTime);
        animator.SetFloat(MoveYHash, input.y, dampTime, Time.deltaTime);
    }

    #endregion

    #region Actions & States

    public void TriggerJump()
    {
        animator.SetTrigger(JumpTriggerHash);
    }

    public void SetDashing(bool isDashing)
    {
        animator.SetBool(DashingHash, isDashing);
    }

    public void SetFalling(bool isFalling)
    {
        animator.SetBool(IsFallingHash, isFalling);
    }

    public void SetCarrying(bool isCarrying)
    {
        animator.SetBool(IsCarryingHash, isCarrying);
    }

    public void SetHit(bool isHit)
    {
        animator.SetBool(IsHitHash, isHit);
    }

    public void SetParry(bool isParry)
    {
        animator.SetBool(IsParryingHash, isParry);
    }

    #endregion

    #region Combat Logic

    public void SetAttackState(bool isAttacking, int comboIndex = 0)
    {
        animator.SetBool(IsAttackingHash, isAttacking);
        animator.SetInteger(ComboIndexHash, comboIndex);
    }

    public void ExitAttack()
    {
        animator.SetBool(IsAttackingHash, false);
        animator.SetInteger(ComboIndexHash, 0);
    }
    

    #endregion

    #region Queries

    public bool IsInAttackAnimation() => animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    public bool IsInHitAnimation() => animator.GetCurrentAnimatorStateInfo(0).IsTag("Hit");
    
    public bool IsLandingFinished()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return !stateInfo.IsName("Land") || stateInfo.normalizedTime >= 1.0f;
    }

    #endregion
}