using System;
using UnityEngine;

public class TriggerCam : MonoBehaviour
{
    #region Link

        [SerializeField] private Camera cam;
        [SerializeField] private Transform camPoint;
        [Range(0f, 360f)] [SerializeField] private float angleToFace;
        
    #endregion
    
    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            cam.transform.position = camPoint.transform.position;
            cam.transform.rotation = Quaternion.Euler(cam.transform.rotation.x, angleToFace, cam.transform.rotation.z);
        }
    }
}
