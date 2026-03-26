using UnityEngine;

public class BumperLogic : MonoBehaviour
{
    [SerializeField] private float bounceForce = 15f;
    [SerializeField] private Transform parentTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;
        
        var controller = other.GetComponentInParent<PlayerController>();
        var manager = other.GetComponentInParent<PlayerManager>();
        
        if (controller == null || manager == null || controller.Rb == null) return;
        
        
        controller.SetJumping(true);
        
        controller.Rb.linearVelocity = new Vector3(0, 0, 0);
        
        controller.Rb.AddForce(parentTransform.up * bounceForce, ForceMode.Impulse);
        
        manager.TransitionTo(manager.JumpState);
    }
}