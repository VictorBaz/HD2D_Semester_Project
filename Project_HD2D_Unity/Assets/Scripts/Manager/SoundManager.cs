using com.Victor.Utilities.Scripts;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace Script.Manager
{
    public class SoundManager : MonoBehaviour
    {
        #region Fields
        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        [Header("Settings")]
        [Range(0, 1)] public float masterVolume = 1f;
        
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        
        [Header("Data")]
        [SerializeField] private AudioDataBank dataBank;

        private Coroutine musicFadeCoroutine;
        #endregion

        #region Singleton
        public static SoundManager Instance;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);

            ConfigureSource(sfxSource, sfxMixerGroup);
            ConfigureSource(musicSource, musicMixerGroup);
        }
        #endregion

        private void ConfigureSource(AudioSource source, AudioMixerGroup group)
        {
            if (source == null) return;
            source.spatialBlend = 0f; 
            source.outputAudioMixerGroup = group;
            source.playOnAwake = false;
        }

        #region Music Logic

        public void PlayMusic(AudioClip newClip, float fadeDuration = 1.5f)
        {
            if (musicSource.clip == newClip && musicSource.isPlaying) return;

            if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
            musicFadeCoroutine = StartCoroutine(MusicTransitionCoroutine(newClip, fadeDuration));
        }

        private IEnumerator MusicTransitionCoroutine(AudioClip newClip, float duration)
        {
            if (musicSource.isPlaying)
            {
                yield return StartCoroutine(musicSource.FadeVolume(0, duration / 2));
            }

            musicSource.Stop();
            musicSource.clip = newClip;
            musicSource.volume = 0;
            musicSource.Play();

            yield return StartCoroutine(musicSource.FadeVolume(masterVolume, duration / 2));
            musicFadeCoroutine = null;
        }
        #endregion

        #region SFX Logic

        public void PlaySfx(SoundType type)
        {
            var entry = dataBank.GetSFX(type);
            if (entry == null || entry.clip == null) return;

            AudioSource targetSource = sfxSource;

            if (entry.useRandomPitch)
            {
                targetSource.PlayWithRandomPitch(entry.clip, 
                    entry.pitch - entry.pitchRandomness, 
                    entry.pitch + entry.pitchRandomness);
            }
            else
            {
                targetSource.pitch = entry.pitch;
                targetSource.PlayOneShot(entry.clip, entry.volume * masterVolume);
            }
        }

        public GameObject PlayTempAudio(AudioClip clip, string name, float volume = 1f)
        {
            GameObject go = new GameObject("TempAudio_" + name);
            go.transform.SetParent(transform);
            
            AudioSource source = go.AddComponent<AudioSource>();
            ConfigureSource(source, sfxMixerGroup);
            
            source.clip = clip;
            source.volume = volume * masterVolume;
            source.Play();

            Destroy(go, clip.length / source.pitch);
            return go;
        }
        #endregion

        public void UpdateMasterVolume(float volume)
        {
            masterVolume = volume;
            if (musicSource != null) musicSource.volume = masterVolume;
            if (sfxSource != null) sfxSource.volume = masterVolume;
        }
    }
}