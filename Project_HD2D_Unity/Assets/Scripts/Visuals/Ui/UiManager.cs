using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class UiManager : MonoBehaviour
{
    #region Variables
    
    private static UiManager Instance;

    [Header("State Panels")]
    [SerializeField] private CanvasGroup pauseMenuPanel;
    [SerializeField] private CanvasGroup mainMenuPanel;
    [SerializeField] private CanvasGroup hudPanel;
    
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

    [Header("X Button Images")]
    [SerializeField] private Image playerLockXButtonImage;
    [SerializeField] private Image playerNotLockXButtonImage;
    
    [Header("A Button Images")]
    [SerializeField] private Image playerLockAButtonImage;
    [SerializeField] private Image playerNotLockAButtonImage;
    
    [Header("Loading Settings")]
    [SerializeField] private CanvasGroup loadingPanel;
    [SerializeField] private RectTransform loadingIcon;
    [SerializeField] private float rotationSpeed = 200f;
    
    private float openLeftPanelX;
    private float openRightPanelX;
    
    private bool isPanelVisible = true;
    
    private bool playerLock;
    private bool lastPlayerLock;
    
    private Tween rotationTween;
    
    private Coroutine focusRetryCoroutine;
    #endregion

    #region Lifecycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        
        openLeftPanelX = canvasGroupLeftPanel.transform.localPosition.x;
        openRightPanelX = canvasGroupRightPanel.transform.localPosition.x;
        
        ForceState(false);
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        EventManager.OnGameStateChanged += HandleUiState;
        EventManager.OnEnergyChanged += HandleEnergyUpdate; 
        EventManager.OnSapChanged += HandleSapUpdate;
        EventManager.OnLockStateChanged += HandleLockUpdate;
        EventManager.OnToggleInputPanel += DisplayPanelInput;
        
    }
    
    private void OnDisable()
    {
        EventManager.OnGameStateChanged -= HandleUiState;
        EventManager.OnEnergyChanged -= HandleEnergyUpdate;
        EventManager.OnSapChanged -= HandleSapUpdate;
        EventManager.OnLockStateChanged -= HandleLockUpdate;
        EventManager.OnToggleInputPanel -= DisplayPanelInput;
        
    }

    private void OnDestroy()
    {
        transform.DOKill(true);
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


    public void UpdateLockState()
    {
        if (EventManager.OnRequestIsPlayerLock == null) return;
        
        bool currentLock = EventManager.OnRequestIsPlayerLock.Invoke();

        if (currentLock == lastPlayerLock && Time.time > 0.1f) return; 
        lastPlayerLock = currentLock;

        float lockAlpha = currentLock ? 1f : 0f;
        float unlockAlpha = currentLock ? 0f : 1f;

        AnimateButtonSwap(playerLockXButtonImage, playerNotLockXButtonImage, lockAlpha, unlockAlpha);
        AnimateButtonSwap(playerLockAButtonImage, playerNotLockAButtonImage, lockAlpha, unlockAlpha);
    }

    private void AnimateButtonSwap(Image lockImg, Image unlockImg, float lockTarget, float unlockTarget)
    {
        lockImg.DOKill();
        unlockImg.DOKill();

        lockImg.DOFade(lockTarget, transitionDuration).SetEase(Ease.InOutQuad);
        unlockImg.DOFade(unlockTarget, transitionDuration).SetEase(Ease.InOutQuad);
    }

    #endregion

    #region Lambda

    private void HandleEnergyUpdate(int curr, int max) => UpdateEnergyDisplay(curr);
    private void HandleSapUpdate(int curr, int max) => UpdateSapDisplay(curr);
    private void HandleLockUpdate(bool isLocked) => UpdateLockState();

    #endregion
    
    #region Handle State

    private void HandleUiState(GameState state)
    {
        float duration = transitionDuration;
    
        if (focusRetryCoroutine != null) StopCoroutine(focusRetryCoroutine);

        switch (state)
        {
            case GameState.Menu:
                ToggleCanvasGroup(mainMenuPanel, true, duration);
                ToggleCanvasGroup(pauseMenuPanel, false, duration);
                ToggleCanvasGroup(hudPanel, false, duration);
            
                GameObject menuButton = mainMenuPanel.GetComponentInChildren<ButtonMenuHandler>().gameObject;
                focusRetryCoroutine = StartCoroutine(EnsureFocusRoutine(menuButton));
                break;

            case GameState.Game:
                ToggleCanvasGroup(mainMenuPanel, false, duration);
                ToggleCanvasGroup(pauseMenuPanel, false, duration);
                ToggleCanvasGroup(hudPanel, true, duration);
                EventSystem.current.SetSelectedGameObject(null);
                break;

            case GameState.Pause:
                ToggleCanvasGroup(mainMenuPanel, false, duration);
                ToggleCanvasGroup(pauseMenuPanel, true, duration);
                ToggleCanvasGroup(hudPanel, true, duration, 0.4f); 
            
                GameObject pauseButton = pauseMenuPanel.GetComponentInChildren<ButtonPauseHandler>().gameObject;
                focusRetryCoroutine = StartCoroutine(EnsureFocusRoutine(pauseButton));
                break;
        }
    }
    
    private IEnumerator EnsureFocusRoutine(GameObject target)
    {
        while (EventSystem.current.currentSelectedGameObject != target)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(target);

            if (EventSystem.current.currentSelectedGameObject == target)
                yield break;

            
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private void ToggleCanvasGroup(CanvasGroup group, bool show, float duration, float targetAlpha = 1f)
    {
        group.DOKill();
        group.blocksRaycasts = show;
        group.interactable = show;
        group.DOFade(show ? targetAlpha : 0f, duration).SetUpdate(true);
    }
    

    #endregion

  
}
