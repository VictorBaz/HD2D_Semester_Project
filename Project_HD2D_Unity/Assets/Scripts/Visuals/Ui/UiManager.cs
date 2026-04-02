using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class UiManager : MonoBehaviour
{
    [Header("Energy Settings")]
    [SerializeField] private Transform energyContainer; 
    [SerializeField] private GameObject energyPointPrefab; 
    
    private List<Image> energyIcons = new List<Image>();


    public void SetupEnergyBar(int maxEnergy)
    {
        ClearEnergyBar();
        CreateEnergyIcons(maxEnergy);
    }

    public void UpdateEnergyDisplay(int currentEnergy)
    {
        for (int i = 0; i < energyIcons.Count; i++)
        {
            UpdateIconState(i, currentEnergy);
        }
    }


    private void ClearEnergyBar()
    {
        foreach (Transform child in energyContainer) 
        {
            child.DOKill(); 
            Destroy(child.gameObject);
        }
        energyIcons.Clear();
    }

    private void CreateEnergyIcons(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(energyPointPrefab, energyContainer);
            ConfigureIcon(obj, i);
        }
    }

    private void ConfigureIcon(GameObject obj, int index)
    {
        if (obj.TryGetComponent(out Image img))
        {
            img.raycastTarget = false; 
            energyIcons.Add(img);
            PlaySpawnAnimation(obj.transform, index);
        }
    }

    private void UpdateIconState(int index, int currentEnergy)
    {
        Image icon = energyIcons[index];
        bool shouldBeActive = (index < currentEnergy);

        if (icon.enabled != shouldBeActive)
        {
            if (shouldBeActive) 
                AnimateEnergyGain(icon);
            else 
                AnimateEnergyLoss(icon);
        }
    }


    private void PlaySpawnAnimation(Transform target, int index)
    {
        target.localScale = Vector3.zero;
        target.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack)
            .SetDelay(index * 0.1f);
    }

    private void AnimateEnergyLoss(Image icon)
    {
        icon.transform.DOPunchRotation(new Vector3(0, 0, 15), 0.3f);
        icon.DOFade(0.2f, 0.2f).OnComplete(() => {
            icon.enabled = false;
        });
    }

    private void AnimateEnergyGain(Image icon)
    {
        icon.enabled = true;
        icon.transform.DOScale(1.2f, 0.1f).OnComplete(() => {
            icon.transform.DOScale(1.0f, 0.1f);
        });
        icon.DOFade(1f, 0.2f);
    }
}