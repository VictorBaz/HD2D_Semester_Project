using System.Collections.Generic;
using Script.Manager;

public class BattleMusicTracker
{
    private readonly SoundManager _soundManager;
    private readonly HashSet<EnemyBaseManager> _activeEnemies = new HashSet<EnemyBaseManager>();

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
            _soundManager.PlayMusic(MusicType.Fight_1);
        }
        else
        {
            _soundManager.PlayMusic(MusicType.Main);
        }
    }
}
