using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Script.Manager;

public class BattleMusicTracker
{
    private readonly SoundManager _soundManager;
    private readonly HashSet<EnemyBaseManager> _activeEnemies = new HashSet<EnemyBaseManager>();

    private bool _isMusicLocked = false;
    private bool _isBattleMusicPlaying = false;
    
    private const float MinBattleDuration = 25f; 

    public BattleMusicTracker(SoundManager soundManager)
    {
        _soundManager = soundManager;
    }

    public void RegisterEnemy(EnemyBaseManager enemy)
    {
        if (_activeEnemies.Add(enemy))
        {
            EvaluateMusic();
        }
    }

    public void UnregisterEnemy(EnemyBaseManager enemy)
    {
        if (_activeEnemies.Remove(enemy))
        {
            EvaluateMusic();
        }
    }

    private void EvaluateMusic()
    {
        if (_soundManager == null) return;

        if (_activeEnemies.Count > 0)
        {
            if (!_isBattleMusicPlaying)
            {
                _soundManager.PlayMusic(MusicType.Fight_1);
                _isBattleMusicPlaying = true;
                _isMusicLocked = true;

                _ = StartMusicCooldownAsync();
            }
        }
        else 
        {
            if (_isMusicLocked) return;

            if (_isBattleMusicPlaying)
            {
                _soundManager.PlayMusic(MusicType.Main);
                _isBattleMusicPlaying = false;
            }
        }
    }

    private async Task StartMusicCooldownAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(MinBattleDuration));
        _isMusicLocked = false;
        EvaluateMusic();
    }
}