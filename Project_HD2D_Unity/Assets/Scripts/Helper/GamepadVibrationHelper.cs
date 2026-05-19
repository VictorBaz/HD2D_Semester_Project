using UnityEngine;
using UnityEngine.InputSystem;

public static class GamepadVibrationHelper
{
    private static Gamepad activeGamepad;
    private static float vibrationEndTime;
    private static bool isVibrating;

    public static void Vibrate(float lowFrequency, float highFrequency, float duration)
    {
        activeGamepad = Gamepad.current;
        if (activeGamepad == null) return;

        activeGamepad.SetMotorSpeeds(lowFrequency, highFrequency);
        
        vibrationEndTime = Time.unscaledTime + duration;

        if (!isVibrating)
        {
            isVibrating = true;
            InputSystem.onAfterUpdate += MonitorVibration;
        }
    }
    public static void StopAllVibrations()
    {
        if (isVibrating)
        {
            InputSystem.onAfterUpdate -= MonitorVibration;
            isVibrating = false;
        }

        Gamepad.current?.SetMotorSpeeds(0f, 0f);
        activeGamepad = null;
    }

    private static void MonitorVibration()
    {
        if (activeGamepad == null)
        {
            StopAllVibrations();
            return;
        }

        if (Time.unscaledTime >= vibrationEndTime) StopAllVibrations();
    }
}