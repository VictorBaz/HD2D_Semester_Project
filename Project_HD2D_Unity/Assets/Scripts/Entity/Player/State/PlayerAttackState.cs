using System.Collections;
using System.Collections.Generic;
using Script.Manager;
using UnityEngine;

namespace Player.State
{
    public class PlayerAttackState : PlayerBaseState
    {
        #region Variables

        public override string Name   => "Attack Melee";
        public override bool   CanMove => true;

        private bool      bufferNextAttack;
        private bool      bufferWindowOpen;
        private int       comboIndex;
        private Coroutine currentAttackRoutine;
        private readonly HashSet<IDamageable> _hitThisCombo = new();
        #endregion

        #region Base State Methods

        public override void EnterState(PlayerStateContext psc)
        {
            _hitThisCombo.Clear();
            comboIndex       = 0;
            bufferNextAttack = false;

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

        public override void FixedUpdateState(PlayerStateContext psc)
        {
            HandlePhysics(psc, 0.45f);
        }

        public void BufferAttack()
        {
            if (bufferWindowOpen)
                bufferNextAttack = true;
        }

        #endregion
        

        private void StartAttackSequence(PlayerStateContext psc)
        {
            _hitThisCombo.Clear();
            
            if (currentAttackRoutine != null)
                psc.Controller.StopCoroutine(currentAttackRoutine);

            psc.AnimationManager.SetAttackState(true, comboIndex);
            currentAttackRoutine = psc.Controller.RunRoutine(AttackMeleeIe(psc));
            
            SoundManager.Instance?.PlaySfx(GetSoundAttack(comboIndex));
        }

        private void RotateTowardsInput(PlayerStateContext psc)
        {
            CalculateTargetDirection(psc);

            if (psc.TargetDirection.magnitude > 0.1f)
                psc.PlayerTransform.forward = psc.TargetDirection;
        }

        private IEnumerator AttackMeleeIe(PlayerStateContext psc)
        {
            CombatHitData hit            = psc.PlayerData.ComboHits[comboIndex];
            float         animLength     = psc.PlayerData.GetAttackClipLength(comboIndex);
            Vector3       dashDir        = psc.PlayerTransform.forward;
            bool          hitboxIsActive = false;
            float         elapsed        = 0f;

            bufferWindowOpen = true;
            
            psc.VfxManagerPlayer.PlayFxCombo(comboIndex);
            
            float actionDuration = Mathf.Max(
                animLength,
                hit.DashStartOffset    + hit.DashDuration,
                hit.HitboxStartOffset  + hit.HitboxActiveDuration);

            while (elapsed < actionDuration)
            {
                elapsed += Time.deltaTime;

                UpdateDashVelocity(psc, hit, dashDir, elapsed);
                UpdateHitbox(psc, hit, elapsed);
                yield return null;
            }

            psc.Rb.linearVelocity = Vector3.zero;

            yield return new WaitForSeconds(0.1f);
            bufferWindowOpen = false;
            ResolveCombo(psc);
        }

        private void UpdateDashVelocity(PlayerStateContext psc, CombatHitData hit, Vector3 dashDir, float elapsed)
        {
            bool inDashWindow = elapsed >= hit.DashStartOffset && elapsed <= hit.DashStartOffset + hit.DashDuration;

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

        private void UpdateHitbox(PlayerStateContext psc, CombatHitData hit, float elapsed)
        {
            bool shouldBeActive = elapsed >= hit.HitboxStartOffset &&
                                  elapsed <= hit.HitboxStartOffset + hit.HitboxActiveDuration;

            if (!shouldBeActive) return;

            int count = psc.Controller.OverlapAttack(psc.PlayerData.LayerEnemy);

            for (int i = 0; i < count; i++)
            {
                var damageable = psc.Controller.HitBuffer[i].GetComponent<IDamageable>();   
                if (damageable == null || _hitThisCombo.Contains(damageable)) continue;

                if (damageable is IDamageableEnemy enemy)
                    enemy.TakeDamage(2, psc.PlayerTransform.forward, comboIndex);
                else
                    damageable.TakeDamage(2, psc.PlayerTransform.forward);

                _hitThisCombo.Add(damageable);
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

        private SoundType GetSoundAttack(int comboIndex)
        {
            return comboIndex switch
            {
                0 => SoundType.Combo_Woosh_1,
                1 => SoundType.Combo_Woosh_2,
                2 => SoundType.Combo_Woosh_3,
                _ => SoundType.Combo_Woosh_1
            };
        }
        
        public int ComboIndex => comboIndex;
    }
}