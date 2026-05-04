using UnityEngine;

namespace Demos_GD.Scripts
{
    public class TeleporterPlayer : MonoBehaviour
    {
        [SerializeField] private Transform teleportPoint;
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.position = teleportPoint.position;
            }
        } 
    }
}
