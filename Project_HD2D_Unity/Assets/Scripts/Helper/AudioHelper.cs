using System.Collections;
using UnityEngine;

namespace com.Victor.Utilities.Scripts
{
    /// <summary>
    /// Fonctions utilitaires pour la gestion audio.
    /// </summary>
    public static class AudioHelper
    {
        /// <summary>
        /// Fait une transition progressive du volume d'une AudioSource.
        /// </summary>
        /// <param name="source">L'AudioSource à modifier</param>
        /// <param name="targetVolume">Le volume cible (0 à 1)</param>
        /// <param name="duration">La durée de la transition en secondes</param>
        public static IEnumerator FadeVolume(this AudioSource source, float targetVolume, float duration)
        {
            float startValue = source.volume;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                source.volume = Mathf.Lerp(startValue, targetVolume, progress);
                yield return null;
            }

            source.volume = targetVolume;
        }

        /// <summary>
        /// Joue un clip audio aléatoire parmi une liste.
        /// </summary>
        /// <param name="source">L'AudioSource qui jouera le son</param>
        /// <param name="clips">Le tableau de clips audio</param>
        public static void PlayRandomSound(AudioSource source, AudioClip[] clips)
        {
            if (clips.Length == 0) return;
            
            source.PlayOneShot(clips[Random.Range(0,clips.Length)]);
        }

        /// <summary>
        /// Joue un clip audio avec un pitch aléatoire pour ajouter de la variété.
        /// </summary>
        /// <param name="source">L'AudioSource qui jouera le son</param>
        /// <param name="clip">Le clip audio à jouer</param>
        /// <param name="minPitch">Le pitch minimum</param>
        /// <param name="maxPitch">Le pitch maximum</param>
        public static void PlayWithRandomPitch(this AudioSource source, AudioClip clip, float minPitch, float maxPitch)
        {
            source.pitch = Random.Range(minPitch, maxPitch);
            source.PlayOneShot(clip);
            source.pitch = 1f;
        }
    }
}