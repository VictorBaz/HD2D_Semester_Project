using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioDataBank", menuName = "Audio/AudioDataBank")]
public class AudioDataBank : ScriptableObject
{
    [System.Serializable]
    public class AudioEntry
    {
        public SoundType type;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 2f)] public float pitch = 1f;
        public bool useRandomPitch = false;
        [Range(0f, 0.5f)] public float pitchRandomness = 0.1f;
    }

    [SerializeField] private List<AudioEntry> sfxEntries;
    [SerializeField] private List<AudioEntry> musicEntries;

    private Dictionary<SoundType, AudioEntry> sfxCache;
    private Dictionary<MusicType, AudioEntry> musicCache;

    public AudioEntry GetSFX(SoundType type)
    {
        if (sfxCache == null) BuildCache();
        return sfxCache.GetValueOrDefault(type);
    }

    private void BuildCache()
    {
        sfxCache = new Dictionary<SoundType, AudioEntry>();
        foreach (var entry in sfxEntries) sfxCache[entry.type] = entry;
    }
}
