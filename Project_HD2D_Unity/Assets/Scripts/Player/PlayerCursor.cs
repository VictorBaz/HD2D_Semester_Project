using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float yOffset;
    [SerializeField] private float deadzone = 0.1f;

    

    void FixedUpdate() {
        Vector3 newPos = player.position + new Vector3(0, yOffset, 0);
        transform.position = Vector3.Lerp(transform.position, newPos, 1f);
    }

    public void HandleRotation(Vector2 shootInput)
    {
        if (shootInput.magnitude < deadzone) return;

        float targetAngle = Mathf.Atan2(shootInput.x, shootInput.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90, targetAngle, 0);
    }
}