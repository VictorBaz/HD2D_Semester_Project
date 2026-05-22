using System.Collections;
using System.Collections.Generic;
using Script.Manager;
using UnityEngine;

namespace Player.State
{
    public class PlayerAttackState : PlayerBaseState
    {
        #region Variables

        public override string Name    => "Attack Melee";
        public override bool   CanMove => true;
        public override bool   CanDash => canDash;
        public int             ComboIndex => comboIndex;

        private bool      bufferNextAttack;
        private bool      bufferWindowOpen;
        private bool      canDash;
        private int       comboIndex;
        private Coroutine currentAttackRoutine;

        private readonly HashSet<IDamageable> hitThisCombo = new();

        #endregion

        #region Base State

        public override void EnterState(PlayerStateContext psc)
        {
            hitThisCombo.Clear();
            comboIndex       = 0;
            bufferNextAttack = false;
            canDash          = false;

            psc.Controller.SetGravity(false);
            StartAttackSequence(psc);
        }

        public override void ExitState(PlayerStateContext psc)
        {
            if (currentAttackRoutine != null)
                psc.Controller.StopCoroutine(currentAttackRoutine);

            bufferWindowOpen = false;
            bufferNextAttack = false;

            psc.Controller.SetGravity(true);
            psc.AnimationManager.ExitAttack();
        }

        public override void UpdateState(PlayerStateContext psc)
        {
            HandleAnimation(psc);
            HandleMovement(psc);
        }

        public override void FixedUpdateState(PlayerStateContext psc) => HandlePhysics(psc, 0.45f);

        public void BufferAttack()
        {
            if (bufferWindowOpen)
                bufferNextAttack = true;
        }

        #endregion

        #region Attack Sequence

        private void StartAttackSequence(PlayerStateContext psc)
        {
            hitThisCombo.Clear();
            canDash = false;

            if (currentAttackRoutine != null)
                psc.Controller.StopCoroutine(currentAttackRoutine);

            psc.AnimationManager.SetAttackState(true, comboIndex);
            currentAttackRoutine = psc.Controller.RunRoutine(AttackMeleeIe(psc));

            SoundManager.Instance?.PlaySfx(GetSoundAttack(comboIndex));
        }

        private IEnumerator AttackMeleeIe(PlayerStateContext psc)
        {
            CombatHitData hit     = psc.PlayerData.ComboHits[comboIndex];
            float         length  = psc.PlayerData.GetAttackClipLength(comboIndex);
            Vector3       dashDir = psc.PlayerTransform.forward;
            float         elapsed = 0f;
            bool          hitboxFired = false;

            bufferWindowOpen = true;
            psc.VfxManagerPlayer.PlayFxCombo(comboIndex);

            while (elapsed < length)
            {
                elapsed += Time.deltaTime;

                if (!canDash && elapsed >= length * 0.8f)
                    canDash = true;

                if (!hitboxFired && elapsed >= hit.HitboxStartOffset)
                {
                    hitboxFired = true;
                    FireHitbox(psc, hit);
                }

                UpdateDashVelocity(psc, hit, dashDir, elapsed);

                yield return null;
            }

            psc.Rb.linearVelocity = Vector3.zero;
            bufferWindowOpen      = false;

            ResolveCombo(psc);
        }

        private void UpdateDashVelocity(PlayerStateContext psc, CombatHitData hit, Vector3 dashDir, float elapsed)
        {
            bool inDashWindow = elapsed >= hit.DashStartOffset &&
                                elapsed <= hit.DashStartOffset + hit.DashDuration;

            if (inDashWindow)
            {
                float t = (elapsed - hit.DashStartOffset) / hit.DashDuration;
                psc.Rb.linearVelocity = Vector3.Lerp(dashDir * hit.DashSpeed, Vector3.zero, t);
            }
            else
            {
                psc.Rb.linearVelocity = Vector3.zero;
            }
        }

        //TODO CHANGE HARD CODED ATTACK VALUE
        private void FireHitbox(PlayerStateContext psc, CombatHitData hit)
        {
            int count = psc.Controller.OverlapAttack(psc.PlayerData.LayerEnemy);

            for (int i = 0; i < count; i++)
            {
                var damageable = psc.Controller.HitBuffer[i].GetComponent<IDamageable>();
                if (damageable == null || hitThisCombo.Contains(damageable)) continue;

                if (damageable is IDamageableEnemy enemy)
                    enemy.TakeDamageIndex(2, psc.PlayerTransform.forward, comboIndex);
                else
                    damageable.TakeDamage(2, psc.PlayerTransform.forward);

                hitThisCombo.Add(damageable);
            }
        }

        private void ResolveCombo(PlayerStateContext psc)
        {
            if (bufferNextAttack && comboIndex < psc.PlayerData.ComboHits.Length - 1)
            {
                comboIndex++;
                bufferNextAttack = false;
                StartAttackSequence(psc);
            }
            else
            {
                DetermineState(psc);
            }
        }

        private static SoundType GetSoundAttack(int index) => index switch
        {
            1 => SoundType.Combo_Woosh_2,
            2 => SoundType.Combo_Woosh_3,
            _ => SoundType.Combo_Woosh_1
        };

        #endregion
    }
}