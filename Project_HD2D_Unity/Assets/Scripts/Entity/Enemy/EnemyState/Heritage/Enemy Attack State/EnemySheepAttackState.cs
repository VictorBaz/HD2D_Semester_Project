using System.Collections;
using UnityEngine;

public class EnemySheepAttackState : EnemyAttackState
{
    protected override IEnumerator AttackSequence(EnemyContext actx)
    {
        var data = actx.Data;
        
        actx.AnimManager.TriggerAttack();
        yield return new WaitForSeconds(data.AnticipationTime);

        CanBeParry = true;
        actx.AnimManager.ToggleAttackCollider(true);

        float elapsed = 0f;
        Vector3 strikeDir = actx.Manager.transform.forward;
        float activePhaseDuration = Mathf.Max(data.AttackDashDuration, data.HitboxActiveDuration);

        while (elapsed < activePhaseDuration)
        {
            elapsed += Time.deltaTime;
            if (elapsed <= data.AttackDashDuration)
                actx.Manager.transform.position += strikeDir * data.AttackDashSpeed * Time.deltaTime;

            if (elapsed >= data.HitboxActiveDuration)
                actx.AnimManager.ToggleAttackCollider(false);

            yield return null;
        }

        actx.AnimManager.ToggleAttackCollider(false);
        yield return new WaitForFixedUpdate();
        CanBeParry = false;

        isCooldown = true;
        yield return new WaitForSeconds(data.AttackCooldown);
        isCooldown = false;
        attackRoutine = null;
    }

    public override void ExitState(EnemyContext actx)
    {
        base.ExitState(actx);
        actx.AnimManager.ToggleAttackCollider(false);
    }
}