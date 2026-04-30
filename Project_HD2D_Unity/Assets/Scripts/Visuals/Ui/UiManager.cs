using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Script.Manager;
using TMPro;

public class UiManager : MonoBehaviour
{
    #region Variables

    public static UiManager Instance;

    [Header("State Panels")]
    [SerializeField] private CanvasGroup pauseMenuPanel;
    [SerializeField] private CanvasGroup mainMenuPanel;
    [SerializeField] private CanvasGroup hudPanel;
    [SerializeField] private CanvasGroup creditsPanel;
    [SerializeField] private CanvasGroup settingsPanel;

    [Header("Energy Settings")]
    [SerializeField] private Transform energyContainer;
    [SerializeField] private GameObject energyPointPrefab;
    [SerializeField] private List<Image> energyIcons;
    [SerializeField] private Sprite energyFullSprite;
    [SerializeField] private Sprite energyEmptySprite;

    [Header("Sap Settings")]
    [SerializeField] private TMP_Text sapCountText;

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
    [SerializeField] private CanvasGroup blackScreenGroup;
    
    [Header("Pop Up")]
    [SerializeField] private CanvasGroup popupGroup;
    private Sequence popupSequence;

    private float openLeftPanelX;
    private float openRightPanelX;

    private bool isPanelVisible = true;
    private bool lastPlayerLock;

    private Tween rotationTween;
    private Coroutine focusRetryCoroutine;

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
        };

        ForceState(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        EventManager.OnGameStateChanged += HandleUiState;
        UiEvents.OnEnergyChanged += HandleEnergyUpdate;
        UiEvents.OnSapChanged += HandleSapUpdate;
        UiEvents.OnLockStateChanged += HandleLockUpdate;
        UiEvents.OnToggleInputPanel += DisplayPanelInput;
        UiEvents.OnEnergySetup += SetupEnergy;
        EventManager.OnLoadingStarted += HandleLoadingStarted;
        EventManager.OnLoadingFinished += HandleLoadingFinished;
        UiEvents.OnShowPopup += ShowPopup;
    }

    private void OnDisable()
    {
        EventManager.OnGameStateChanged -= HandleUiState;
        UiEvents.OnEnergyChanged -= HandleEnergyUpdate;
        UiEvents.OnSapChanged -= HandleSapUpdate;
        UiEvents.OnLockStateChanged -= HandleLockUpdate;
        UiEvents.OnToggleInputPanel -= DisplayPanelInput;
        UiEvents.OnEnergySetup -= SetupEnergy;
        EventManager.OnLoadingStarted -= HandleLoadingStarted;
        EventManager.OnLoadingFinished -= HandleLoadingFinished;
        UiEvents.OnShowPopup -= ShowPopup;
    }

    private void OnDestroy()
    {
        transform.DOKill(true);
    }

    #endregion

    #region Energy Logic

    private void UpdateEnergyDisplay(int currentEnergy)
    {
        for (int i = 0; i < energyIcons.Count; i++)
        {
            bool shouldBeFull = (i < currentEnergy);
            bool isFull = energyIcons[i].sprite == energyFullSprite;

            if (isFull == shouldBeFull) continue;

            if (shouldBeFull)
                AnimateGain(energyIcons[i], energyFullSprite);
            else
                AnimateLoss(energyIcons[i], energyEmptySprite);
        }
    }

    private void SetupEnergy(int maxEnergy, int currentEnergy)
    {
        foreach (Transform child in energyContainer)
            Destroy(child.gameObject);
        
        energyIcons.Clear();

        for (int i = 0; i < maxEnergy; i++)
        {
            GameObject obj = Instantiate(energyPointPrefab, energyContainer);
            if (!obj.TryGetComponent(out Image img)) continue;

            img.raycastTarget = false;
            img.sprite = i < currentEnergy ? energyFullSprite : energyEmptySprite;
            energyIcons.Add(img);
        }
    }

    #endregion

    #region Animations

    private void AnimateLoss(Image icon, Sprite sprite)
    {
        icon.transform.DOPunchRotation(new Vector3(0, 0, 15), 0.3f);
        icon.DOFade(0f, 0.15f).OnComplete(() =>
        {
            icon.sprite = sprite;
            icon.DOFade(1f, 0.15f);
        });
    }

    private void AnimateGain(Image icon, Sprite sprite)
    {
        icon.transform.DOKill();
        icon.DOFade(0f, 0.15f).OnComplete(() =>
        {
            icon.sprite = sprite;
            icon.DOFade(1f, 0.15f);
            icon.transform.DOScale(1.2f, 0.1f).OnComplete(() => icon.transform.DOScale(1.0f, 0.1f));
        });
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

    private void AnimateButtonSwap(Image lockImg, Image unlockImg, float lockTarget, float unlockTarget)
    {
        lockImg.DOKill();
        unlockImg.DOKill();

        lockImg.DOFade(lockTarget, transitionDuration).SetEase(Ease.InOutQuad);
        unlockImg.DOFade(unlockTarget, transitionDuration).SetEase(Ease.InOutQuad);
    }

    #endregion

    #region Event Handlers

    private void HandleEnergyUpdate(int curr, int max) => UpdateEnergyDisplay(curr);

    private void HandleSapUpdate(int curr)
    {
        if (sapCountText != null)
            sapCountText.text = curr.ToString();
    }
    

    private void HandleLockUpdate(bool isLocked)
    {
        if (isLocked == lastPlayerLock && Time.time > 0.1f) return;
        lastPlayerLock = isLocked;

        float lockAlpha = isLocked ? 1f : 0f;
        float unlockAlpha = isLocked ? 0f : 1f;

        AnimateButtonSwap(playerLockXButtonImage, playerNotLockXButtonImage, lockAlpha, unlockAlpha);
        AnimateButtonSwap(playerLockAButtonImage, playerNotLockAButtonImage, lockAlpha, unlockAlpha);
    }

    private void HandleLoadingStarted()
    {
        ToggleCanvasGroup(loadingPanel, true, transitionDuration);

        rotationTween?.Kill();
        rotationTween = loadingIcon
            .DORotate(new Vector3(0, 0, -360), 360f / rotationSpeed, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear)
            .SetUpdate(true);
    }

    private void HandleLoadingFinished()
    {
        ToggleCanvasGroup(loadingPanel, false, transitionDuration);

        rotationTween?.Kill();
        rotationTween = null;
    }

    #endregion

    #region Handle State

    private void HandleUiState(GameState state)
    {
        if (focusRetryCoroutine != null) StopCoroutine(focusRetryCoroutine);

        foreach (var kvp in panelMap)
            ToggleCanvasGroup(kvp.Value, kvp.Key == state, transitionDuration);

        if (state == GameState.Pause)
            ToggleCanvasGroup(hudPanel, true, transitionDuration, 0.4f);

        focusRetryCoroutine = GetFocusTarget(state) is GameObject target
            ? StartCoroutine(EnsureFocusRoutine(target))
            : null;

        if (state == GameState.Game)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private GameObject GetFocusTarget(GameState state)
    {
        return state switch
        {
            GameState.Menu     => mainMenuPanel.GetComponentInChildren<ButtonMenuHandler>().gameObject,
            GameState.Pause    => pauseMenuPanel.GetComponentInChildren<ButtonPauseHandler>().gameObject,
            GameState.Credits  => creditsPanel.GetComponentInChildren<Selectable>()?.gameObject,
            GameState.Settings => settingsPanel.GetComponentInChildren<Selectable>()?.gameObject,
            _                  => null
        };
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

    #region GamePlay Related

    public IEnumerator FadeBlackScreen(float targetAlpha, float duration)
    {
        float startAlpha = blackScreenGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            blackScreenGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        blackScreenGroup.alpha = targetAlpha;
    }

    private void ShowPopup()
    {
        float duration = 1.5f;
        
        popupSequence?.Kill();
        
        popupGroup.alpha  = 0f;
        popupGroup.gameObject.SetActive(true);

        popupSequence = DOTween.Sequence()
            .Append(popupGroup.DOFade(1f, 0.4f))
            .AppendInterval(duration)
            .Append(popupGroup.DOFade(0f, 0.4f))
            .OnComplete(() => popupGroup.gameObject.SetActive(false));
        
        SoundManager.Instance?.PlaySfx(SoundType.Pop_Up);
    }
    
    #endregion
}