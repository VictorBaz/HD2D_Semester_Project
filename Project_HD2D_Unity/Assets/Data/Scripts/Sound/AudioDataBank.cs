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

    private Dictionary<SoundType, AudioEntry> _sfxCache;
    private Dictionary<MusicType, AudioEntry> _musicCache;

    public AudioEntry GetSFX(SoundType type)
    {
        if (_sfxCache == null) BuildCache();
        return _sfxCache.GetValueOrDefault(type);
    }

    private void BuildCache()
    {
        _sfxCache = new Dictionary<SoundType, AudioEntry>();
        foreach (var entry in sfxEntries) _sfxCache[entry.type] = entry;
    }
}

public enum SoundType
{
    
    Footstep_Dirt, Footstep_Stone, Jump_Hop, Jump_Land, Dash,
    
    Parry_Activate, Parry_Hit, Parry_Perfect,
    Combo_Woosh_1, Combo_Woosh_2, Combo_Woosh_3,
    Combo_Hit_1_2, Combo_Hit_3,
    
    Enemy_Carry, Enemy_Drop_Throw, Enemy_Ko_Full, Enemy_Stun_Loop, Enemy_Recovery,
    
    Fissure_Lock, Fissure_Energy_In, Fissure_Energy_Out,
    Plant_Grow, Plant_Shrink,
    Carniflore_Open, Carniflore_Close,
    Bouncer_Open, Bouncer_Close, Bouncer_Rebound,
    
    Collect_Prayer, Damage_Taken, Health_Regen,
    Damage_Effective, Damage_Ineffective, Death_Groan,
    Damage_Effective_Giant, Damage_Ineffective_Giant, Death_Groan_Giant
}

public enum MusicType
{
    Menu, Puzzle_Calm, Combat_Dynamic, Level_End, GameOver, Boss_Fight, Tutorial
}