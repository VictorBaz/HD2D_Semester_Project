using System;
using UnityEngine;

public class EnemyAnimationManager : BaseAnimationManager
{
    public Animator Animator => animator;
    
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    
    private static readonly int IsKOHash = Animator.StringToHash("IsKO");
    private static readonly int IsAlreadyKoHash = Animator.StringToHash("IsAlreadyKo");
    
    private static readonly int IsCarryHash = Animator.StringToHash("IsCarry");
    
    private static readonly int IsExposedHash = Animator.StringToHash("IsExposed");
    
    private static readonly int IsChargingHash = Animator.StringToHash("IsCharging");

    [SerializeField] private GameObject colliderAttack;
    [SerializeField] private GameObject colliderAttackEnemy;
    [SerializeField] private GameObject colliderRepulse;

    private void Awake()
    {
        ToggleRepulsiveCollider(false);
        ToggleAttackCollider(false);
    }

    public void UpdateMovement(float currentSpeed) => animator.SetFloat(SpeedHash, currentSpeed);
    public void SetExposed(bool isExposed) => animator.SetBool(IsExposedHash, isExposed);

    public void TriggerAttack() => animator.SetTrigger(IsAttackingHash);
    public void TriggerCharge() => animator.SetTrigger(IsChargingHash);
    
    public void SetCarry(bool isCarry) => animator.SetBool(IsCarryHash,isCarry);

    public void ToggleAttackCollider(bool toggle) => ToggleCollider(toggle, colliderAttack);
    
    public void ToggleRepulsiveCollider(bool active) => ToggleCollider(active, colliderRepulse);
    public void ToggleAttackColliderEnemy(bool active) => ToggleCollider(active, colliderAttackEnemy);

    private void ToggleCollider(bool active, GameObject collider)
    {
        if(collider) collider.SetActive(active);
    }

    public void HandleKo(bool isKO,bool wasKo)
    {
        animator.SetBool(IsKOHash, isKO);
        animator.SetBool(IsAlreadyKoHash, wasKo);
    }
}