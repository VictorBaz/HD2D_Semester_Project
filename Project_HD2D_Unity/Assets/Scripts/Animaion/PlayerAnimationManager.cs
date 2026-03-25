using UnityEngine;

public class PlayerAnimationManager : BaseAnimationManager
{
    private static readonly int MoveXHash = Animator.StringToHash("moveX");
    private static readonly int MoveYHash = Animator.StringToHash("moveY");
    private static readonly int VelocityYHash = Animator.StringToHash("VelocityY");
    private static readonly int JumpTriggerHash = Animator.StringToHash("Jump");
    private static readonly int DashingHash = Animator.StringToHash("Dashing");
    private static readonly int IsParryingHash = Animator.StringToHash("IsParrying");
    private static readonly int ComboIndexHash = Animator.StringToHash("ComboIndex");

    [SerializeField] private float dampTime = 0.1f;

    public void HandleMovement(Vector2 input, float velocityY)
    {
        animator.SetFloat(MoveXHash, input.x, dampTime, Time.deltaTime);
        animator.SetFloat(MoveYHash, input.y, dampTime, Time.deltaTime);
        animator.SetFloat(VelocityYHash, velocityY);
    }

    public void TriggerJump() => animator.SetTrigger(JumpTriggerHash);
    public void SetDashing(bool isDashing) => animator.SetBool(DashingHash, isDashing);
    public void SetParry(bool isParry) => animator.SetBool(IsParryingHash, isParry);
    
    public void SetAttackState(bool isAttacking, int comboIndex = 0)
    {
        animator.SetBool(IsAttackingHash, isAttacking);
        animator.SetInteger(ComboIndexHash, comboIndex);
    }
}