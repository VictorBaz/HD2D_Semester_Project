using UnityEngine;

public class BumperLogic : MonoBehaviour
{
    [SerializeField] private float bounceForce = 15f;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;
        
        var manager = other.GetComponentInParent<PlayerManager>();
        var controller = other.GetComponentInParent<PlayerController>();
        
        if (manager == null || controller == null) return;

        if (controller.Rb == null) return;
        
        if (controller != null) controller.SetJumping(true);

        controller.Rb.linearVelocity = new Vector3(controller.Rb.linearVelocity.x, 0, controller.Rb.linearVelocity.z);
        
        controller.Rb.AddForce(transform.up * bounceForce, ForceMode.Impulse);

        manager.TransitionTo(manager.AirState);
    }
}