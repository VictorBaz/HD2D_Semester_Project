using System.Threading.Tasks;
using UnityEngine;

public static class HitStopHandler
{
    private static bool isWaiting;

    public static async void PlayHitStop(float duration)
    {
        if (isWaiting || duration <= 0) return;

        isWaiting = true;
        
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        await Task.Delay((int)(duration * 1000));

        Time.timeScale = originalTimeScale;
        isWaiting = false;
    }
}