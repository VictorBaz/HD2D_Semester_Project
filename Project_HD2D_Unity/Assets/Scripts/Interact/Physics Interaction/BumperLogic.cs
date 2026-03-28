using UnityEngine;

public class BumperLogic : MonoBehaviour
{
    [SerializeField] private float     bounceForce     = 15f;
    [SerializeField] private Transform parentTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;

        var controller = other.GetComponentInParent<PlayerController>();
        var manager    = other.GetComponentInParent<PlayerManager>();

        if (controller == null || manager == null || controller.Rb == null) return;

        Vector3 bounceDirection = parentTransform.up * bounceForce;
        controller.Rb.linearVelocity = new Vector3(0f, bounceDirection.y, 0f);

        manager.TransitionTo(manager.BumpState);
        controller.SetJumping(true);
    }
}