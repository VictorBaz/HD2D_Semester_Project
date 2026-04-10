using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class UiManager : MonoBehaviour
{
    #region Variables

    [Header("Energy Settings")]
    [SerializeField] private Transform energyContainer; 
    [SerializeField] private GameObject energyPointPrefab; 
    private List<Image> energyIcons = new List<Image>();

    [Header("Sap Settings")]
    [SerializeField] private Transform sapContainer; 
    [SerializeField] private GameObject sapPointPrefab; 
    private List<Image> sapIcons = new List<Image>();
    
    [Header("Panel Settings")]
    [SerializeField] private CanvasGroup canvasGroupLeftPanel;
    [SerializeField] private CanvasGroup canvasGroupRightPanel;
    [SerializeField] private float hideOffset = 200f; 
    [SerializeField] private float transitionDuration = 0.25f;

    private float openLeftPanelX;
    private float openRightPanelX;
    private bool isPanelVisible = true; 

    
    #endregion
    

    #region Lifecycle

    private void Awake()
    {
        openLeftPanelX = canvasGroupLeftPanel.transform.localPosition.x;
        openRightPanelX = canvasGroupRightPanel.transform.localPosition.x;

        ForceState(false);
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }

    #endregion

    #region Energy Logic

    public void SetupEnergyBar(int maxEnergy, int currentEnergy) 
        => SetupBar(energyContainer, energyIcons, energyPointPrefab, maxEnergy, currentEnergy);
    public void UpdateEnergyDisplay(int currentEnergy) => UpdateDisplay(energyIcons, currentEnergy);

    #endregion

    #region Sap Logic

    public void SetupSapBar(int maxSap, int currentSap) 
        => SetupBar(sapContainer, sapIcons, sapPointPrefab, maxSap, currentSap);
    public void UpdateSapDisplay(int currentSap) => UpdateDisplay(sapIcons, currentSap);

    #endregion

    #region Generic Bar Logic 

    private void SetupBar(Transform container, List<Image> icons, GameObject prefab, int maxCount, int currentCount)
    {
        ClearContainer(container, icons);
    
        for (int i = 0; i < maxCount; i++)
        {
            GameObject obj = Instantiate(prefab, container);
            if (obj.TryGetComponent(out Image img))
            {
                img.raycastTarget = false;
                icons.Add(img);
                
                bool isActive = (i < currentCount);
                
                img.enabled = isActive;
                img.color = new Color(img.color.r, img.color.g, img.color.b, isActive ? 1f : 0.2f);
            
                
                if (isActive) 
                {
                    PlaySpawnAnimation(obj.transform, i);
                }
                else 
                {
                    obj.transform.localScale = Vector3.one; 
                }
            }
        }
    }

    private void UpdateDisplay(List<Image> icons, int currentCount)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            Image icon = icons[i];
            bool shouldBeActive = (i < currentCount);

            if (icon.enabled != shouldBeActive)
            {
                if (shouldBeActive) 
                    AnimateGain(icon);
                else 
                    AnimateLoss(icon);
            }
        }
    }

    private void ClearContainer(Transform container, List<Image> icons)
    {
        foreach (Transform child in container) 
        {
            child.DOKill(); 
            Destroy(child.gameObject);
        }
        icons.Clear();
    }

    #endregion

    #region Animations

    private void PlaySpawnAnimation(Transform target, int index)
    {
        target.localScale = Vector3.zero;
        target.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack)
            .SetDelay(index * 0.05f);
    }

    private void AnimateLoss(Image icon)
    {
        icon.transform.DOPunchRotation(new Vector3(0, 0, 15), 0.3f);
        icon.DOFade(0.2f, 0.2f).OnComplete(() => {
            icon.enabled = false;
        });
    }

    private void AnimateGain(Image icon)
    {
        icon.enabled = true;
        icon.transform.DOKill(); 
        icon.transform.DOScale(1.2f, 0.1f).OnComplete(() => {
            icon.transform.DOScale(1.0f, 0.1f);
        });
        icon.DOFade(1f, 0.2f);
    }

    #endregion

    #region Panel Input Display

    private void ForceState(bool on)
    {
        isPanelVisible = on;
        canvasGroupLeftPanel.alpha = on ? 1f : 0f;
        canvasGroupRightPanel.alpha = on ? 1f : 0f;
    
        canvasGroupLeftPanel.transform.localPosition = new Vector3(on ? openLeftPanelX : openLeftPanelX - hideOffset, 0, 0);
        canvasGroupRightPanel.transform.localPosition = new Vector3(on ? openRightPanelX : openRightPanelX + hideOffset, 0, 0);
    }

    public void DisplayPanelInput(bool on)
    {
        if (isPanelVisible == on) return;
        isPanelVisible = on;

        float targetAlpha = on ? 1f : 0f;
        float leftX = on ? openLeftPanelX : openLeftPanelX - hideOffset;
        float rightX = on ? openRightPanelX : openRightPanelX + hideOffset;

        canvasGroupLeftPanel.DOKill();
        canvasGroupRightPanel.DOKill();
        canvasGroupLeftPanel.transform.DOKill();
        canvasGroupRightPanel.transform.DOKill();

        canvasGroupLeftPanel.DOFade(targetAlpha, transitionDuration);
        canvasGroupRightPanel.DOFade(targetAlpha, transitionDuration);

        canvasGroupLeftPanel.transform.DOLocalMoveX(leftX, transitionDuration).SetEase(Ease.OutCubic);
        canvasGroupRightPanel.transform.DOLocalMoveX(rightX, transitionDuration).SetEase(Ease.OutCubic);
    }

    #endregion
}
