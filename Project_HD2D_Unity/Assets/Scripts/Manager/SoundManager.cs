using UnityEngine;
using UnityEngine.Audio;

namespace Script.Manager
{
    public class SoundManager : MonoBehaviour
    {
        #region Fields

        public AudioSource audioSource;
        [SerializeField] private AudioSource musicAudioSource;

        [Range(0,1)] public float masterVolume = 1f;
        
        [Header("Music BPM Settings")]
        [SerializeField] private AudioClip gameMusic;
        
        [Header("Audio Mixer Group")]
        [SerializeField] private AudioMixerGroup _audioMixerGroupMainMusic;

        #endregion

        #region Singleton

        public static SoundManager instance;
        
        #endregion

        #region Unity LifeCycle

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            SetupMusicAudioSource();
            audioSource.volume = masterVolume; 
        }

        #endregion

        #region Music Setup

        private void SetupMusicAudioSource()
        {
            if (musicAudioSource == null)
            {
                GameObject musicObject = new GameObject("MusicAudioSource");
                musicObject.transform.SetParent(transform);
                musicAudioSource = musicObject.AddComponent<AudioSource>();
            }

            musicAudioSource.loop = true;
            musicAudioSource.playOnAwake = false;
            musicAudioSource.volume = masterVolume;
            musicAudioSource.outputAudioMixerGroup = _audioMixerGroupMainMusic;
        }

        public void StartGameMusic()
        {
            if (gameMusic != null && musicAudioSource != null)
            {
                musicAudioSource.clip = gameMusic;
                musicAudioSource.Play();
            }
        }

        public void StopGameMusic()
        {
            if (musicAudioSource != null && musicAudioSource.isPlaying)
            {
                musicAudioSource.Stop();
            }
        }

        public void PauseGameMusic()
        {
            if (musicAudioSource != null && musicAudioSource.isPlaying)
            {
                musicAudioSource.Pause();
            }
        }

        public void ResumeGameMusic()
        {
            if (musicAudioSource != null && !musicAudioSource.isPlaying)
            {
                musicAudioSource.UnPause();
            }
        }
        
        #endregion

        #region Sound Methods

        public void PlaySoundWithAudioSource(AudioSource source, AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("The audioClip you tried to play is null");
                return;
            }
            source.PlayOneShot(clip);
        }
        
        public void PlayMusicOneShot(AudioClip _audioClip)
        {
            if (_audioClip == null)
            {
                Debug.LogError("The audioClip you tried to play is null");
                return;
            }
            audioSource.PlayOneShot(_audioClip);
        }
        
        
        public void UpdateMasterVolume(float volume)
        {
            masterVolume = volume;
            audioSource.volume = masterVolume;
            if (musicAudioSource != null)
            {
                musicAudioSource.volume = masterVolume;
            }
        }

        public GameObject InitialisationAudioObjectDestroyAtEnd(AudioClip audioClipTarget, bool looping, 
            bool playingAwake, float volumeSound, string _name)
        {
            GameObject emptyObject = new GameObject(_name);
            emptyObject.transform.SetParent(gameObject.transform);

            AudioSource audioSourceGeneral = emptyObject.AddComponent<AudioSource>();
            audioSourceGeneral.clip = audioClipTarget;
            audioSourceGeneral.loop = looping;
            audioSourceGeneral.playOnAwake = playingAwake;
            audioSourceGeneral.volume = volumeSound * masterVolume;
            audioSourceGeneral.Play();
            
            if (!looping)
            {
                Destroy(emptyObject, audioClipTarget.length);
            }
            
            return emptyObject;
        }

        #endregion

        
    }
}