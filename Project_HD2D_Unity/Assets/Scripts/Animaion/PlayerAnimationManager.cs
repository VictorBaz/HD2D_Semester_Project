using UnityEngine;

public class PlayerAnimationManager : BaseAnimationManager
{
    private static readonly int MoveXHash = Animator.StringToHash("moveX");
    private static readonly int MoveYHash = Animator.StringToHash("moveY");
    private static readonly int InputMagnitudeHash = Animator.StringToHash("InputMagnitude");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int DashingHash = Animator.StringToHash("Dashing");
    private static readonly int IsCarryingHash = Animator.StringToHash("IsCarrying");
    private static readonly int ComboIndexHash = Animator.StringToHash("ComboIndex");
    private static readonly int IsParryingHash = Animator.StringToHash("IsParrying");
    private static readonly int JumpTriggerHash = Animator.StringToHash("Jump");

    [SerializeField] private float dampTime = 0.1f;

    public void HandleAnimation(float inputRawMagnitude, Vector2 inputBlendTree, bool isGrounded, Vector3 velocity)
    {
        UpdateMovement(inputBlendTree);
        
        animator.SetFloat(InputMagnitudeHash, inputRawMagnitude);
        animator.SetBool(IsGroundedHash, isGrounded);
        //animator.SetFloat(VelocityYHash, velocity.y); 
        
        bool isFalling = !isGrounded && velocity.y < -0.1f;
        animator.SetBool(IsFallingHash, isFalling);
    }

    private void UpdateMovement(Vector2 input)
    {
        animator.SetFloat(MoveXHash, input.x, dampTime, Time.deltaTime);
        animator.SetFloat(MoveYHash, input.y, dampTime, Time.deltaTime);
    }

    public void TriggerJump() => animator.SetTrigger(JumpTriggerHash);
    public void SetDashing(bool isDashing) => animator.SetBool(DashingHash, isDashing);
    public void SetParry(bool isParry) => animator.SetBool(IsParryingHash, isParry);
    public void SetCarrying(bool isCarrying) => animator.SetBool(IsCarryingHash, isCarrying);
    
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
}