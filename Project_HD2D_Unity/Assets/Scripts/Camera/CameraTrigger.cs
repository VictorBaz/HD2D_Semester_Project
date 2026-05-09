using System;
using Enum;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CameraSettings settings;
    [SerializeField] private bool triggerOnlyOnce = false;
    
    private bool hasTriggered = false;
    private bool wasPlayedPermanently = false;

    private void OnTriggerEnter(Collider other)
    {
        if (settings == null || hasTriggered || (triggerOnlyOnce && wasPlayedPermanently)) return;

        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;
        
        hasTriggered = true;
        wasPlayedPermanently = true;
        CameraEvents.TriggerCamera(settings);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null && other.CompareTag(GameConstants.PLAYER_TAG)) 
            hasTriggered = false;
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (!box) return;
        
        Gizmos.matrix = transform.localToWorldMatrix;

        switch (settings.CameraPlayerState)
        {
            case CameraPlayerState.Fix:
                Gizmos.color = new Color(1, 1, 0, 0.5f);
                break;
            case CameraPlayerState.FollowPlayer:
                Gizmos.color = new Color(0.5f, 0.5f, 1, 0.5f);
                break;
            case CameraPlayerState.Cinematic:
                Gizmos.color = new Color(0.5f, 1, 1, 0.5f);
                break;
            case CameraPlayerState.Rail:
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                break;
            default:
                Gizmos.color = new Color(0, 1, 1, 0.2f);
                break;
        }
        
        
        Gizmos.DrawCube(box.center, box.size);
        Gizmos.DrawWireCube(box.center, box.size);
    }

    private void OnDrawGizmosSelected()
    {
        if (settings == null) return;

        if (settings.CameraPlayerState == CameraPlayerState.Cinematic && settings.targetCinematic != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, settings.targetCinematic.position);
            Gizmos.DrawWireSphere(settings.targetCinematic.position, 1f);
            Gizmos.DrawSphere(settings.targetCinematic.position, 0.2f);
        }
        
        if (settings.CameraPlayerState is not (CameraPlayerState.Fix or CameraPlayerState.Cinematic)) return;

        if (settings.CameraTargetTransform == null) return;

        Gizmos.color = settings.CameraPlayerState == CameraPlayerState.Fix ? Color.red : Color.cyan;

        Gizmos.DrawLine(transform.position, settings.CameraTargetTransform.position);
        Gizmos.DrawWireSphere(settings.CameraTargetTransform.position, 1f);
        Gizmos.DrawSphere(settings.CameraTargetTransform.position, 0.2f);

        if (Camera.main != null)
        {
            Transform camRef = Camera.main.transform.parent ? Camera.main.transform.parent : Camera.main.transform;
            Quaternion rotation = camRef.rotation;
            
            Gizmos.matrix = Matrix4x4.TRS(settings.CameraTargetTransform.position, rotation, Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, 60f, 3f, 0.1f, 1.77f);
            
            Gizmos.matrix = Matrix4x4.identity;
        }
        
        if (settings.CameraPlayerState == CameraPlayerState.Rail && settings.ActiveRail != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 firstNode = settings.ActiveRail.Nodes[0];
            Gizmos.DrawLine(firstNode, firstNode + settings.RailOffset);
            Gizmos.DrawWireSphere(firstNode + settings.RailOffset, 0.5f);
        }
    }
}