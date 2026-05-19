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
    private EnergyTrace        energyTrace;

    public ILockable CurrentTarget { get; private set; }
    public bool IsLocked => CurrentTarget != null;

    private readonly List<ILockable> lockableTargets = new List<ILockable>();

    private LayerMask _obstaclesMask;

    #endregion

    #region Init

    public void InitData(PlayerDataInstance data)
    {
        playerData     = data;
        _obstaclesMask = ~(playerData.LockableLayer | playerData.PlayerLayer);
    }

    public void InitManager(PlayerStateContext psc)
    {
        vfxManagerPlayer = psc.VfxManagerPlayer;
        energyTrace = psc.VfxManagerPlayer.EnergyTrace;
    }

    #endregion

    #region Lock Behaviour

    public void CalculLockRotation()
    {
        if (!IsLocked) return;

        if (!IsTargetValid(CurrentTarget) || IsTargetBehindWall(CurrentTarget))
        {
            Unlock();
            return;
        }

        Vector3 directionToTarget = (CurrentTarget.GetLockTransform().position - playerTransform.position).normalized;
        directionToTarget.y = 0;

        if (directionToTarget == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        Quaternion.Slerp(
            playerTransform.rotation,
            targetRotation,
            playerData.RotationSpeed * Time.deltaTime);
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

        if (CurrentTarget is Flaw flaw)
        {
            flaw.OnLockStateChanged(true);
            vfxManagerPlayer.UpdateLinkVisuals(flaw.IsBlocked());
        }

        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySfx(SoundType.Energy_activation);
            SoundManager.Instance.PlayLoopingSfx(SoundType.Fissure_Lock);
        }
    }

    public void Unlock()
    {
        if (SoundManager.Instance && CurrentTarget != null)
        {
            SoundManager.Instance.PlaySfx(SoundType.Energy_desactivation);
            SoundManager.Instance.StopLoopingSfx(SoundType.Fissure_Lock);
        }

        if (CurrentTarget is Flaw flaw)
        {
            flaw.OnLockStateChanged(false);
            vfxManagerPlayer.UpdateLinkVisuals(flaw.IsBlocked());
        }

        CurrentTarget = null;
        vfxManagerPlayer.LinkVfx(false);
    }

    #endregion

    #region Lock Algorithm

    private void FindLockableTargets()
    {
        lockableTargets.Clear();

        List<ILockable> candidates = DetectionHelper.FindVisibleTargets<ILockable>(
            playerTransform,
            playerData.LockRange,
            playerData.LockAngle,
            playerData.LockableLayer);

        foreach (ILockable lockable in candidates)
        {
            if (lockable.IsLockable() && !IsTargetBehindWall(lockable))
                lockableTargets.Add(lockable);
        }
    }

    private ILockable GetBestLockableTarget(List<ILockable> targets)
    {
        ILockable bestTarget = null;
        float     bestScore  = float.MaxValue;

        foreach (ILockable target in targets)
        {
            if (!IsTargetValid(target)) continue;

            Transform t   = target.GetLockTransform();
            float distance = Vector3.Distance(playerTransform.position, t.position);
            float angle    = Vector3.Angle(playerTransform.forward,
                                 (t.position - playerTransform.position).normalized);

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

        return DetectionHelper.IsInDistance(
            playerTransform,
            target.GetLockTransform(),
            playerData.LockRange);
    }

    private bool IsTargetBehindWall(ILockable target)
    {
        Transform targetTransform = target.GetLockTransform();
        Vector3   origin          = playerTransform.position;
        Vector3   direction       = (targetTransform.position - origin).normalized;
        float     distance        = Vector3.Distance(origin, targetTransform.position);

        return Physics.Raycast(origin, direction, distance, _obstaclesMask);
    }

    #endregion
}