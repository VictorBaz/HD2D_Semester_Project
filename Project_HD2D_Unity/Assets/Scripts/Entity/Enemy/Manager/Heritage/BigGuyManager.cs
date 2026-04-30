using UnityEngine;

public class BigGuyManager : EnemyBaseManager
{
    
    protected override void InitializeState()
    {
        AttackState = new BigGuyJumpAttackState();
        DropState = new BigGuyDropState();
    }

    protected override void Start()
    {
        base.Start();
        context.SetVisualParam(GameConstants.PARAM_SHEEP_SHADER_NAME,0,1);
    }
}