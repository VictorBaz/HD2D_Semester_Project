using UnityEngine;

public class BigGuyManager : EnemyBaseManager
{

    protected override void InitializeAttackState()
    {
        AttackState = new BigGuyJumpAttackState();
    }

    protected override void Start()
    {
        base.Start();
        context.SetVisualParam(GameConstants.PARAM_SHEEP_SHADER_NAME,0,1);
    }
}