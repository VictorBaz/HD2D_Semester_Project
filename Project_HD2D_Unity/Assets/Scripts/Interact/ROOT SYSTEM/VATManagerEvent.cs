using UnityEngine;
using UnityEngine.Events;

public class VATManagerEvent : VATManager
{
    [Header("Threshold Settings")]
    [Tooltip("L'index du palier dans AnimationSteps qui déclenche l'événement")]
    [SerializeField] private int activationStepIndex = 3;

    [Header("Events")]
    public UnityEvent OnThresholdReached;
    public UnityEvent OnThresholdExited;

    private bool isThresholdActive = false;

    protected override void OnValueUpdated(float newValue)
    {
        float threshold = animationSteps[Mathf.Clamp(activationStepIndex, 0, MaxEnergyIndex)];

        switch (isThresholdActive)
        {
            case false when newValue >= (threshold - 0.01f):
                isThresholdActive = true;
                OnThresholdReached?.Invoke();
                break;
            case true when newValue < (threshold - 0.01f):
                isThresholdActive = false;
                OnThresholdExited?.Invoke();
                break;
        }
    }
}