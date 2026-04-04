using UnityEngine;

public abstract class CameraTriggerBase : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";
    
    private bool hasTriggered = false;
    
    protected virtual Color GizmoColor => new Color(1, 1, 1, 0.2f);
    protected virtual string Name => "Camera Trigger Base Gizmo";

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        
        if (other.CompareTag(PLAYER_TAG))
        {
            hasTriggered = true;
            Trigger();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            hasTriggered = false;
        }
    }
    
    protected abstract void Trigger();
    
    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        
        if (box == null) return;
        
        Gizmos.matrix = transform.localToWorldMatrix;
            
        Gizmos.color = GizmoColor;
        Gizmos.DrawCube(box.center, box.size);

        Gizmos.color = new Color(GizmoColor.r, GizmoColor.g, GizmoColor.b, 1f);
        Gizmos.DrawWireCube(box.center, box.size);

        Gizmos.matrix = Matrix4x4.identity;
            
        //Gizmos.DrawIcon(transform.position + new Vector3(0,1,0), Name, true);
    }
}