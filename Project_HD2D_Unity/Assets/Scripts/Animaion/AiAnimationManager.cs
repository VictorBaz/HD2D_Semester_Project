using UnityEngine;

public class AiAnimationManager : BaseAnimationManager
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsKOHash = Animator.StringToHash("IsKO");
    private static readonly int IsExposedHash = Animator.StringToHash("IsExposed");

    [SerializeField] private GameObject colliderAttack;

    public void UpdateMovement(float currentSpeed) => animator.SetFloat(SpeedHash, currentSpeed);
    public void SetKO(bool isKO) => animator.SetBool(IsKOHash, isKO);
    public void SetExposed(bool isExposed) => animator.SetBool(IsExposedHash, isExposed);

    public void TriggerAttack() => animator.SetTrigger(IsAttackingHash);

    public void ToggleAttackCollider(bool toggle)
    {
        if (colliderAttack != null) colliderAttack.SetActive(toggle);
    }
}