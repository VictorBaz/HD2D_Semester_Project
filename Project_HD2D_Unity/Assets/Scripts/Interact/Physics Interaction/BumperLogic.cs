using System;
using UnityEngine;

public class BumperLogic : MonoBehaviour
{
    [SerializeField] private float     bounceForce     = 15f;
    [SerializeField] private Transform parentTransform;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;

        var controller = other.GetComponentInParent<PlayerController>();
        var manager    = other.GetComponentInParent<PlayerManager>();

        if (controller == null || manager == null || controller.Rb == null) return;

        Vector3 bounceVelocity = parentTransform.up * bounceForce;
    
        controller.Rb.linearVelocity = bounceVelocity;

        manager.TransitionTo(manager.BumpState);
        controller.SetJumping(true);
    }

    #region Debug Gizmos

    private void OnDrawGizmos()
    {
        if (parentTransform == null) return;

        Gizmos.color = Color.yellow;

        Vector3 start = parentTransform.position;
        Vector3 direction = parentTransform.up * (bounceForce * 0.3f);
        Vector3 end = start + direction;

        Gizmos.DrawLine(start, end);

        Gizmos.DrawWireSphere(end, 0.2f);

        Gizmos.matrix = parentTransform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(1f, 0.1f, 1f));
    }

    #endregion
}