using System.Collections.Generic;
using UnityEngine;

public static class DetectionHelper
{
    /// <summary>
    /// Détecte et trie des objets implémentant une interface T dans un rayon et un angle donnés.
    /// </summary>
    public static List<T> FindVisibleTargets<T>(
        Transform origin, 
        float range, 
        float angle, 
        LayerMask layerMask) where T : class
    {
        List<T> visibleTargets = new List<T>();
        
        Collider[] colliders = Physics.OverlapSphere(origin.position, range, layerMask);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out T component))
            {
                
                Vector3 directionToTarget = (collider.transform.position - origin.position).normalized;
                float angleToTarget = Vector3.Angle(origin.forward, directionToTarget);

                if (angleToTarget <= angle)
                {
                    visibleTargets.Add(component);
                }
            }
        }

        return visibleTargets;
    }

    /// <summary>
    /// Retourne la meilleure cible parmi une liste en calculant un score (Distance + Angle).
    /// </summary>
    public static T GetBestTarget<T>(
        Transform origin, 
        List<T> targets, 
        float angleWeight = 0.1f) where T : class
    {
        if (targets == null || targets.Count == 0) return null;

        T bestTarget = null;
        float bestScore = float.MaxValue;

        foreach (T target in targets)
        {
            Transform targetTransform = (target as Component)?.transform;
            if (targetTransform == null) continue;

            float distance = Vector3.Distance(origin.position, targetTransform.position);
            Vector3 direction = (targetTransform.position - origin.position).normalized;
            float angle = Vector3.Angle(origin.forward, direction);

            float score = distance + (angle * angleWeight);

            if (score < bestScore)
            {
                bestScore = score;
                bestTarget = target;
            }
        }

        return bestTarget;
    }

    /// <summary>
    /// Vérifie si une cible est toujours dans la portée.
    /// </summary>
    public static bool IsInDistance(Transform origin, Transform target, float maxDistance)
    {
        if (target == null) return false;
        return Vector3.Distance(origin.position, target.position) <= maxDistance;
    }
}