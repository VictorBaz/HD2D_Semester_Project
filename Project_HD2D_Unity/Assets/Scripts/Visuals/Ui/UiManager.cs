using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Script.Manager;
using TMPro;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    #region Variables

    public static UiManager Instance;

    [Header("UI Elements")]
    [SerializeField] private Slider lifeSlider;
    [SerializeField] private Slider lifeSliderBackground;
    
    [Header("State Panels")]
    [SerializeField] private CanvasGroup pauseMenuPanel;
    [SerializeField] private CanvasGroup mainMenuPanel;
    [SerializeField] private CanvasGroup hudPanel;
    [SerializeField] private CanvasGroup creditsPanel;
    [SerializeField] private CanvasGroup settingsPanel;
    [Header("End Game Credits Settings")]
    [SerializeField] private CanvasGroup endGameCreditsPanel;
    [SerializeField] private RectTransform creditsScrollingImage;

    [Header("Energy Settings")]
    [SerializeField] private Image energyFillImage;
    [SerializeField] private Image energyFillImageBackground;
    private const float MaxEnergyShaderValue = 1f;
    private Tween energyFillTween;
    private Tween energyFillBackgroundTween;
    private float currentEnergyFill;
    private float currentEnergyFillBackground;

    [Header("Sap Settings")]
    [SerializeField] private TMP_Text sapCountText;

    [Header("Panel Settings")]
    [SerializeField] private CanvasGroup canvasGroupLeftPanel;
    [SerializeField] private CanvasGroup canvasGroupRightPanel;
    [SerializeField] private float hideOffset = 200f;
    [SerializeField] private float transitionDuration = 0.25f;

    [Header("Loading Settings")]
    [SerializeField] private CanvasGroup loadingPanel;
    [SerializeField] private RectTransform loadingIcon;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private CanvasGroup blackScreenGroup;
    [SerializeField] private float loadingFadeDuration = 0.4f;

    [Header("Pop Up")]
    [SerializeField] private CanvasGroup popupGroup;

    [SerializeField] private Image pressX;
    
    [Header("Area Notification")]
    [SerializeField] private CanvasGroup areaPanelGroup;
    [SerializeField] private TMP_Text areaNameText;
    [SerializeField] private float areaDisplayDuration = 2f;
    
    [Header("Sprite Popup Settings")]
    [SerializeField] private CanvasGroup spritePopupGroup;
    [SerializeField] private Image spritePopupImage;
    [SerializeField] private float spriteSwitchInterval = 0.3f; 
    
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
    private List<Tween> activeTweens = new List<Tween>();
    
    private Sequence spritePopupSequence;
    private Sequence spritePressXSequence;
    private Tween spriteAnimationTween;
    private List<Sprite> activePopupSprites;
    private int currentSpriteIndex;
    
    private Sequence areaSequence;
    private Sequence popupSequence;
    private Tween creditsScrollTween;

    private float openLeftPanelX;
    private float openRightPanelX;

    private bool isPanelVisible = true;
    private bool lastPlayerLock;

    private Tween rotationTween;
    private Coroutine focusRetryCoroutine;
    
    private bool isEventsSubscribed = false;

    private Dictionary<GameState, CanvasGroup> panelMap;

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

        panelMap = new Dictionary<GameState, CanvasGroup>
        {
            { GameState.Menu,     mainMenuPanel  },
            { GameState.Credits,  creditsPanel   },
            { GameState.Settings, settingsPanel  },
            { GameState.Pause,    pauseMenuPanel },
            { GameState.Game,     hudPanel       },
            { GameState.FinalCredits, endGameCreditsPanel}
        };

        ForceState(false);

        if (endGameCreditsPanel != null)
        {
            endGameCreditsPanel.alpha = 0f;
            endGameCreditsPanel.blocksRaycasts = false;
            endGameCreditsPanel.interactable = false;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void Start()
    {
        if (masterVolumeSlider && masterVolumeSlider.TryGetComponent<SliderElementHandler>(out var masterHandler))
        {
            masterHandler.OnValueChangedAction = OnMasterVolumeChanged;
        }

        if (musicVolumeSlider && musicVolumeSlider.TryGetComponent<SliderElementHandler>(out var musicHandler))
        {
            musicHandler.OnValueChangedAction = OnMusicVolumeChanged;
        }

        if (sfxVolumeSlider && sfxVolumeSlider.TryGetComponent<SliderElementHandler>(out var sfxHandler))
        {
            sfxHandler.OnValueChangedAction = OnSfxVolumeChanged;
        }
    }

    private void OnEnable()
    {
        if (isEventsSubscribed) return;
        
        EventManager.OnGameStateChanged += HandleUiState;
        UiEvents.OnEnergyChanged += HandleEnergyUpdate;
        UiEvents.OnSapChanged += HandleSapUpdate;
        UiEvents.OnToggleInputPanel += DisplayPanelInput;
        EventManager.OnLoadingStarted += HandleLoadingStarted;
        EventManager.OnLoadingFinished += HandleLoadingFinished;
        UiEvents.OnShowPopup += ShowPopup;
        UiEvents.OnShowArea += ShowAreaNotification;
        UiEvents.OnShowSpritePopup += HandleShowSpritePopup;
        UiEvents.OnHideSpritePopup += HandleHideSpritePopup;
        GameplayEvents.OnCredits += StartEndGameCredits;

        SceneManager.sceneLoaded += HandleUiStatePatchForUnity;
        
        isEventsSubscribed = true;
    }

    private void OnDisable()
    {
        if (!isEventsSubscribed) return;
        
        EventManager.OnGameStateChanged -= HandleUiState;
        UiEvents.OnEnergyChanged -= HandleEnergyUpdate;
        UiEvents.OnSapChanged -= HandleSapUpdate;
        UiEvents.OnToggleInputPanel -= DisplayPanelInput;
        EventManager.OnLoadingStarted -= HandleLoadingStarted;
        EventManager.OnLoadingFinished -= HandleLoadingFinished;
        UiEvents.OnShowPopup -= ShowPopup;
        UiEvents.OnShowArea -= ShowAreaNotification;
        UiEvents.OnShowSpritePopup -= HandleShowSpritePopup;
        UiEvents.OnHideSpritePopup -= HandleHideSpritePopup;
        GameplayEvents.OnCredits -= StartEndGameCredits;
        
        SceneManager.sceneLoaded -= HandleUiStatePatchForUnity;
        
        StopFocusRoutine();
        
        isEventsSubscribed = false;
    }

    private void OnDestroy()
    {
        KillAllTweens();
        
        StopFocusRoutine();
    }

    #endregion

    #region Dangerous: Tween Lifecycle Management

    private T RegisterTween<T>(T tween) where T : Tween
    {
        if (tween == null) return null;
        activeTweens.Add(tween);
        return tween;
    }
    
    private void KillAllTweens()
    {
        foreach (var tween in activeTweens)
        {
            tween?.Kill();
        }
        activeTweens.Clear();
        
        transform.DOKill(true);
    }

    #endregion

    #region Dangerous: Coroutine Lifecycle Management

    private void StopFocusRoutine()
    {
        if (focusRetryCoroutine != null)
        {
            StopCoroutine(focusRetryCoroutine);
            focusRetryCoroutine = null;
        }
    }

    #endregion

    #region Panel Input Display

    private void ForceState(bool on)
    {
        isPanelVisible = on;
        canvasGroupLeftPanel.alpha = on ? 1f : 0f;
        canvasGroupRightPanel.alpha = on ? 1f : 0f;

        canvasGroupLeftPanel.transform.localPosition = 
            new Vector3(on ? openLeftPanelX : openLeftPanelX - hideOffset, -87.5f, 0);
        canvasGroupRightPanel.transform.localPosition = 
            new Vector3(on ? openRightPanelX : openRightPanelX + hideOffset, 0, 0);
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

        RegisterTween(canvasGroupLeftPanel.DOFade(targetAlpha, transitionDuration));
        RegisterTween(canvasGroupRightPanel.DOFade(targetAlpha, transitionDuration));

        RegisterTween(canvasGroupLeftPanel.transform.DOLocalMoveX(leftX, transitionDuration).SetEase(Ease.OutCubic));
        RegisterTween(canvasGroupRightPanel.transform.DOLocalMoveX(rightX, transitionDuration).SetEase(Ease.OutCubic));
    }

    private void AnimateButtonSwap(Image lockImg, Image unlockImg, float lockTarget, float unlockTarget)
    {
        lockImg.DOKill();
        unlockImg.DOKill();

        RegisterTween(lockImg.DOFade(lockTarget, transitionDuration).SetEase(Ease.InOutQuad));
        RegisterTween(unlockImg.DOFade(unlockTarget, transitionDuration).SetEase(Ease.InOutQuad));
    }

    #endregion

    #region Event Handlers

    private void HandleEnergyUpdate(int curr, int max)
    {
        if (energyFillImage == null || energyFillImageBackground == null) 
            return;

        float targetFill = (max > 0) ? (MaxEnergyShaderValue / max) * curr : 0f;
        bool isDecreasing = targetFill < currentEnergyFill;

        energyFillTween?.Kill();
        energyFillTween = RegisterTween(DOTween.To(
                () => currentEnergyFill,
                x => { currentEnergyFill = x; energyFillImage.materialForRendering.SetFloat("_fillAmount", x); },
                targetFill,
                0.7f)
            .SetEase(Ease.InOutCubic));

        energyFillBackgroundTween?.Kill();
        energyFillBackgroundTween = RegisterTween(DOTween.To(
                () => currentEnergyFillBackground,
                x => { currentEnergyFillBackground = x; energyFillImageBackground.materialForRendering.SetFloat("_fillAmount", x); },
                targetFill,
                isDecreasing ? 0.8f : 0.6f)
            .SetDelay(isDecreasing ? 0.3f : 0f)
            .SetEase(isDecreasing ? Ease.OutCubic : Ease.InOutCubic));
    }

    private void HandleSapUpdate(int curr)
    {
        if (sapCountText != null)
            sapCountText.text = curr.ToString();
    }

    private void HandleLoadingStarted()
    {
        if (loadingIcon == null || loadingPanel == null) 
            return;

        if (blackScreenGroup != null)
        {
            blackScreenGroup.DOKill();
            blackScreenGroup.blocksRaycasts = true;
            RegisterTween(blackScreenGroup.DOFade(1f, loadingFadeDuration)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    ToggleCanvasGroup(loadingPanel, true, transitionDuration);
                }));
        }
        else
        {
            ToggleCanvasGroup(loadingPanel, true, transitionDuration);
        }

        rotationTween?.Kill();
        rotationTween = null;
        
        rotationTween = RegisterTween(loadingIcon
            .DORotate(new Vector3(0, 0, -360), 360f / rotationSpeed, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear)
            .SetUpdate(true));
    }

    private void HandleLoadingFinished()
    {
        if (loadingPanel == null) 
            return;

        ToggleCanvasGroup(loadingPanel, false, transitionDuration);

        rotationTween?.Kill();
        rotationTween = null;

        if (blackScreenGroup != null)
        {
            blackScreenGroup.DOKill();
            RegisterTween(blackScreenGroup.DOFade(0f, loadingFadeDuration)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    if (blackScreenGroup != null)
                        blackScreenGroup.blocksRaycasts = false;
                }));
        }
    }

    #endregion

    #region Handle State

    private void HandleUiStatePatchForUnity(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == GameManager.Instance.GameName)
        {
            foreach (var kvp in panelMap)
                ToggleCanvasGroup(kvp.Value, kvp.Key == GameState.Game, transitionDuration);
        }
    }

    private void HandleUiState(GameState state)
    {
        StopFocusRoutine();

        if (state != GameState.Game)
        {
            foreach (var kvp in panelMap)
                ToggleCanvasGroup(kvp.Value, kvp.Key == state, transitionDuration);
        }
        
        if (state == GameState.Pause)
        {
            ToggleCanvasGroup(hudPanel, false, transitionDuration, 0f);
        }

        GameObject target = GetFocusTarget(state);
        if (target != null)
        {
            focusRetryCoroutine = StartCoroutine(EnsureFocusRoutine(target));
        }

        if (state == GameState.Game)
        {
            EventSystem.current.SetSelectedGameObject(null);
            ToggleCanvasGroup(mainMenuPanel, false, 0.1f, 0f);
            ToggleCanvasGroup(pauseMenuPanel, false, 0.1f, 0f);
            ToggleCanvasGroup(hudPanel, true, 0.1f);
        }
    }

    private GameObject GetFocusTarget(GameState state)
    {
        return state switch
        {
            GameState.Menu     => mainMenuPanel.GetComponentInChildren<ButtonMenuHandler>()?.gameObject,
            GameState.Pause    => pauseMenuPanel.GetComponentInChildren<ButtonPauseHandler>()?.gameObject,
            GameState.Credits  => creditsPanel.GetComponentInChildren<Selectable>()?.gameObject,
            GameState.Settings => settingsPanel.GetComponentInChildren<Selectable>()?.gameObject,
            _                  => null
        };
    }

    private IEnumerator EnsureFocusRoutine(GameObject target)
    {
        if (target == null)
            yield break;

        while (target != null && EventSystem.current.currentSelectedGameObject != target)
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
        if (group == null) return;
        
        group.DOKill();
        group.blocksRaycasts = show;
        group.interactable   = show;
        RegisterTween(group.DOFade(show ? targetAlpha : 0f, duration).SetUpdate(true));
    }

    #endregion

    #region GamePlay Related

    public IEnumerator FadeBlackScreen(float targetAlpha, float duration)
    {
        if (blackScreenGroup == null)
            yield break;

        float startAlpha = blackScreenGroup.alpha;
        float elapsed    = 0f;

        while (elapsed < duration && blackScreenGroup != null)
        {
            elapsed += Time.deltaTime;
            blackScreenGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        if (blackScreenGroup != null)
            blackScreenGroup.alpha = targetAlpha;
    }

    private void ShowPopup()
    {
        if (popupGroup == null)
            return;

        float duration = 1.5f;

        popupSequence?.Kill();

        popupGroup.alpha = 0f;
        popupGroup.gameObject.SetActive(true);

        popupSequence = RegisterTween(DOTween.Sequence()
            .Append(popupGroup.DOFade(1f, 0.4f))
            .AppendInterval(duration)
            .Append(popupGroup.DOFade(0f, 0.4f))
            .OnComplete(() => {
                if (popupGroup != null)
                    popupGroup.gameObject.SetActive(false);
            }));

        SoundManager.Instance?.PlaySfx(SoundType.Pop_Up);
    }

    private void ShowAreaNotification(string areaName)
    {
        if (areaPanelGroup == null || areaNameText == null) 
            return;

        areaSequence?.Kill();
        areaNameText.text = areaName;

        areaPanelGroup.alpha = 0f;
        areaNameText.transform.localScale = Vector3.one * 0.9f; 
        areaPanelGroup.gameObject.SetActive(true);

        areaSequence = RegisterTween(DOTween.Sequence());

        areaSequence.Append(areaPanelGroup.DOFade(1f, 1.2f).SetEase(Ease.OutSine))
            .Join(areaNameText.transform.DOScale(1f, 1.2f).SetEase(Ease.OutSine));

        areaSequence.AppendInterval(areaDisplayDuration);

        areaSequence.Append(areaPanelGroup.DOFade(0f, 1.2f).SetEase(Ease.InSine))
            .Join(areaNameText.transform.DOScale(0.9f, 1.2f).SetEase(Ease.InSine));
    
        areaSequence.OnComplete(() => {
            if (areaPanelGroup != null)
                areaPanelGroup.gameObject.SetActive(false);
        });
    }

    #endregion

    #region Life UI

    public void SetupLifeUi(float maxLife, float currentLife)
    {
        lifeSlider.maxValue            = maxLife;
        lifeSlider.value               = currentLife;
        lifeSliderBackground.maxValue  = maxLife;
        lifeSliderBackground.value     = currentLife;
    }

    public void UpdateLifeUi(float value)
    {
        this.UpdateSlider(lifeSlider, value, 0.5f);
        this.UpdateSlider(lifeSliderBackground, value, 0.7f);
    }

    #endregion

    #region Tutorial UI

    private void HandleShowSpritePopup(List<Sprite> sprites)
    {
        if (spritePopupGroup == null || spritePopupImage == null) 
            return;

        activePopupSprites = sprites;
        currentSpriteIndex = 0;

        spritePopupImage.sprite = activePopupSprites[0];
        spritePopupGroup.DOKill();
        spritePopupSequence?.Kill();
        spriteAnimationTween?.Kill();

        spritePopupGroup.gameObject.SetActive(true);

        spritePopupGroup.alpha = 0f;
        spritePopupGroup.transform.localScale = Vector3.one * 0.8f;
        
        spritePressXSequence = RegisterTween(DOTween.Sequence()
            .Append(pressX.rectTransform.DOScaleX(1.05f, 0.5f).SetEase(Ease.InOutQuad))
            .SetLoops(-1, LoopType.Yoyo));
        
        spritePopupSequence = RegisterTween(DOTween.Sequence()
            .Append(spritePopupGroup.DOFade(1f, transitionDuration).SetEase(Ease.OutQuad))
            .Join(spritePopupGroup.transform.DOScale(1f, transitionDuration).SetEase(Ease.OutBack))
            .OnComplete(StartSpriteAnimation));
        
    }

    private void HandleHideSpritePopup()
    {
        if (spritePopupGroup == null) 
            return;

        spritePopupGroup.DOKill();
        spritePopupSequence?.Kill();
        spriteAnimationTween?.Kill();

        spritePopupSequence = RegisterTween(DOTween.Sequence()
            .Append(spritePopupGroup.DOFade(0f, transitionDuration).SetEase(Ease.InQuad))
            .Join(spritePopupGroup.transform.DOScale(0.8f, transitionDuration).SetEase(Ease.InQuad))
            .OnComplete(() => {
                if (spritePopupGroup != null)
                    spritePopupGroup.gameObject.SetActive(false);
            }));
    }

    private void StartSpriteAnimation()
    {
        if (activePopupSprites == null || activePopupSprites.Count <= 1) 
            return;

        spriteAnimationTween = RegisterTween(DOVirtual.Float(0, 1, spriteSwitchInterval, (value) => { })
            .SetLoops(-1, LoopType.Restart)
            .OnStepComplete(() =>
            {
                if (activePopupSprites == null || spritePopupImage == null)
                    return;

                currentSpriteIndex = (currentSpriteIndex + 1) % activePopupSprites.Count;
                spritePopupImage.sprite = activePopupSprites[currentSpriteIndex];
            }));
    }

    #endregion
    
    #region Settings Audio Callbacks
    public void OnMasterVolumeChanged(float value)
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.UpdateMasterVolume(value);
        }
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.UpdateMusicVolume(value);
        }
    }

    public void OnSfxVolumeChanged(float value)
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.UpdateSfxVolume(value);
        }
    }
    #endregion

    [SerializeField] private float startingYCredits;
    [SerializeField] private float endingYCredits;
    
    private void StartEndGameCredits(float duration)
    {
        if (endGameCreditsPanel == null || creditsScrollingImage == null) 
            return;

        creditsScrollTween?.Kill();
        endGameCreditsPanel.DOKill();
        creditsScrollingImage.DOKill();

        ToggleCanvasGroup(hudPanel, false, transitionDuration);
        ToggleCanvasGroup(endGameCreditsPanel, true, transitionDuration);
        
        creditsScrollingImage.anchoredPosition = new Vector2(creditsScrollingImage.anchoredPosition.x, startingYCredits);

        float targetY = endingYCredits;

        creditsScrollTween = RegisterTween(creditsScrollingImage.DOAnchorPosY(targetY, duration)
            .SetEase(Ease.Linear)
            .SetUpdate(true));
    }
}