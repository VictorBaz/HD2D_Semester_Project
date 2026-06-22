using System;
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
        context.SetVisualParam(GameConstants.PARAM_SHEEP_SHADER_NAME,0,GameConstants.INDEX_MATERIAL_PULSE);
    }

    private void OnCollisionEnter(Collision other)
    {
        //fucking inshalla kill quick if suck
        if (CurrentState is BigGuyJumpAttackState)
        {
            ChangeState(PatrolState);
        }
    }
}