using UnityEngine;
using UnityEngine.UI;


public class UiManager : MonoBehaviour
{
    [SerializeField] private Slider lifeBarSlider;

    private void UpdateLifeBar(float newValue)
    {
        this.UpdateSlider(lifeBarSlider, newValue);
    }
}
