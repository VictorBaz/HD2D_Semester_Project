using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UiManager : MonoBehaviour
{
    #region Variables
    
    [SerializeField] private Slider lifeBarSlider;
    [SerializeField] private Slider energyBarSlider;
    [SerializeField] private TMP_Text energyText;

    #endregion

    #region Helper
    
    private void UpdateText(TMP_Text tmpText, string text)
    {
        tmpText.text = text;
    }

    #endregion

    #region Utility
    
    private void UpdateLifeBar(float newValue)
    {
        this.UpdateSlider(lifeBarSlider, newValue);
    }

    public void UpdateEnergyTxt(float newValue) =>
        UpdateText(energyText, newValue.ToString());


    public void UpdateEnergyBar(float newValue)
    {
        energyBarSlider.value = newValue;
    }
    
    #endregion
    
}
