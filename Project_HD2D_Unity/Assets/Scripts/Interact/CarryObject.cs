using Interface;
using UnityEngine;

public class CarryObject : MonoBehaviour, ICarryable
{
    [SerializeField] private Rigidbody rb;
    
    public void Carry(Transform playerHead)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;

        transform.SetParent(playerHead);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        
        if (TryGetComponent<Collider>(out var col)) {
            col.enabled = false; 
        }
    }

    public bool IsCarryable() => true;

    public void Eject()
    {
        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = true;
        
        if (TryGetComponent<Collider>(out var col)) 
        {
            col.enabled = true;
        }
        
        rb.AddForce(transform.forward * 5f, ForceMode.Impulse);
    }
}