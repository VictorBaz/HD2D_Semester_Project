using UnityEngine;

public static class DestructionHelper
{
    public static void Explode(GameObject fracturedPrefab, Vector3 position, float force, float radius)
    {
        Rigidbody[] pieces = fracturedPrefab.GetComponentsInChildren<Rigidbody>();
        
        foreach (Rigidbody rb in pieces)
        {
            rb.isKinematic = false;
            
            rb.AddExplosionForce(force, position, radius);
        }
    }
}