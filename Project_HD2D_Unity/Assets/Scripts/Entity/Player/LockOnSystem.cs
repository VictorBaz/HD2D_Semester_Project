using System.Collections.Generic;
using Script.Manager;
using UnityEngine;

public class LockOnSystem : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private Transform playerTransform;

    private PlayerDataInstance playerData;
    private VfxManagerPlayer   vfxManagerPlayer;

    public ILockable CurrentTarget { get; private set; }
    public bool IsLocked => CurrentTarget != null;

    private readonly List<ILockable> lockableTargets = new List<ILockable>();

    #endregion

    #region Init

    public void InitData(PlayerDataInstance data)    => playerData = data;

    public void InitManager(PlayerStateContext psc)  => vfxManagerPlayer = psc.VfxManagerPlayer;

    #endregion

    #region Lock Behaviour

    public Quaternion CalculLockRotation()
    {
        if (!IsLocked) return playerTransform.rotation;

        if (!IsTargetValid(CurrentTarget))
        {
            Unlock();
            return playerTransform.rotation;
        }

        Vector3 directionToTarget = (CurrentTarget.GetLockTransform().position - playerTransform.position).normalized;
        directionToTarget.y = 0;

        if (directionToTarget == Vector3.zero) return playerTransform.rotation;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        return Quaternion.Slerp(
            playerTransform.rotation,
            targetRotation,
            playerData.RotationSpeed * Time.deltaTime
        );
    }

    #endregion

    #region Lock Gates

    public void TryLock()
    {
        FindLockableTargets();
        if (lockableTargets.Count == 0) return;

        CurrentTarget = GetBestLockableTarget(lockableTargets);
        if (CurrentTarget == null) return;

        vfxManagerPlayer.LinkVfx(true, CurrentTarget.GetLockTransform());

        if (CurrentTarget is IEnergyLockable energyLockable)
            energyLockable.OnLockStateChanged(true);

        SoundManager.Instance?.PlaySfx(SoundType.Energy_activation);
        SoundManager.Instance?.PlayLoopingSfx(SoundType.Fissure_Lock);
    }

    public void Unlock()
    {
        if (CurrentTarget is IEnergyLockable energyLockable)
            energyLockable.OnLockStateChanged(false);

        CurrentTarget = null;
        vfxManagerPlayer.LinkVfx(false);

        SoundManager.Instance?.PlaySfx(SoundType.Energy_desactivation);
        SoundManager.Instance?.StopLoopingSfx(SoundType.Fissure_Lock);
    }

    #endregion

    #region Lock Algorithm

    private void FindLockableTargets()
    {
        lockableTargets.Clear();

        Collider[] colliders = Physics.OverlapSphere(
            playerTransform.position,
            playerData.LockRange,
            playerData.LockableLayer);

        foreach (var collider in colliders)
        {
            ILockable lockable = collider.GetComponent<ILockable>();
            if (lockable == null || !lockable.IsLockable()) continue;

            Vector3 direction   = (lockable.GetLockTransform().position - playerTransform.position).normalized;
            float   angleToTarget = Vector3.Angle(playerTransform.forward, direction);

            if (angleToTarget <= playerData.LockAngle)
                lockableTargets.Add(lockable);
        }
    }

    private ILockable GetBestLockableTarget(List<ILockable> targets)
    {
        ILockable bestTarget = null;
        float     bestScore  = float.MaxValue;

        foreach (var target in targets)
        {
            if (!IsTargetValid(target)) continue;

            float distance = Vector3.Distance(playerTransform.position, target.GetLockTransform().position);
            float angle    = Vector3.Angle(playerTransform.forward,
                                 (target.GetLockTransform().position - playerTransform.position).normalized);

            float score = distance + angle * 0.1f;
            if (score >= bestScore) continue;

            bestScore  = score;
            bestTarget = target;
        }

        return bestTarget;
    }

    private bool IsTargetValid(ILockable target)
    {
        if (target == null || !target.IsLockable()) return false;

        return Vector3.Distance(playerTransform.position, target.GetLockTransform().position)
               <= playerData.LockRange;
    }

    #endregion
}