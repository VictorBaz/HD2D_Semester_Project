using System;
using System.Collections;
using UnityEngine;

public class EnemySheepManager : EnemyBaseManager
{
    protected override void InitializeAttackState()
    {
        AttackState = new EnemySheepAttackState();
    }

    protected override void Start()
    {
        base.Start();
        
        context.SetVisualParam(GameConstants.PARAM_SHEEP_SHADER_NAME,0,1);
    }
}