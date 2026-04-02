using UnityEngine;

public class BigGuyManager : EnemyBaseManager
{
    protected override void InitializeAttackState()
    {
        AttackState = new BigGuyJumpAttackState();
    }
    
}