using UnityEngine;

namespace Demos_GD.Scripts
{
    public class SFXPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip clip;
        
        public void PlaySound()
        {
            audioSource.PlayOneShot(clip);
        }
    }
}