using UnityEngine;

namespace Demos_GD.Scripts
{
    public class CameraRotator : MonoBehaviour
    {
        private void Update()
        {
            transform.forward = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
        }
    }
}
