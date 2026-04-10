using System;
using System.Collections;
using UnityEngine;

public class EnemySheepManager : EnemyBaseManager
{
    protected override void InitializeAttackState()
    {
        AttackState = new EnemySheepAttackState();
    }
    
}