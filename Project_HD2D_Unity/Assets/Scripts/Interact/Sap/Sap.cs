using UnityEngine;
using UnityEngine.UI;

public class Sap : MonoBehaviour, ISapLockable, IDataPersistence
{
    [SerializeField] private EntityID   entityID;
    [SerializeField] private GameObject vfxSapPresent;
    [SerializeField] private Slider     progressSlider;
    [SerializeField] private float      collectDuration = 2f;

    private bool  _isEmpty;
    private bool  _playerInZone;
    private float _timer;

    #region Unity Lifecycle

    private void Awake()
    {
        _timer = 0f;
        progressSlider.gameObject.SetActive(false);
        progressSlider.value = 0f;
        progressSlider.maxValue = collectDuration;
    }

    private void Update()
    {
        if (_isEmpty || (_timer <= 0f && !_playerInZone)) return;

        TickTimer();
        UpdateSlider();
        CheckCollect();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (_isEmpty) return;
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;
        
        _playerInZone = true;
        progressSlider.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;

        _playerInZone = false;
    }

    #endregion

    #region Collect

    private void Collect()
    {
        _isEmpty      = true;
        _playerInZone = false;
        _timer        = 0f;

        progressSlider.gameObject.SetActive(false);
        progressSlider.value = 0f;

        vfxSapPresent.SetActive(false);
        UiEvents.TriggerShowPopup();

        var player = PlayerEvents.OnRequestPlayerContext?.Invoke();
        player?.PlayerData.AddSap();
        UiEvents.TriggerSapChanged(player?.PlayerData.Sap ?? 0);
    }
    
    private void TickTimer()
    {
        _timer += _playerInZone ? Time.deltaTime : -Time.deltaTime;
        _timer  = Mathf.Clamp(_timer, 0f, collectDuration);
    }

    private void UpdateSlider()
    {
        progressSlider.value = _timer;

        if (!_playerInZone && _timer <= 0f)
            progressSlider.gameObject.SetActive(false);
    }

    private void CheckCollect()
    {
        if (_timer >= collectDuration)
            Collect();
    }

    #endregion

    #region Save

    public void LoadData(GameData data)
    {
        SapSaveData myData = data.sapDataList.Find(x => x.id == entityID.ID);
        if (myData == null) return;

        _isEmpty = myData.isEmpty;
        vfxSapPresent.SetActive(!_isEmpty);

        if (_isEmpty) progressSlider.gameObject.SetActive(false);
    }

    public void SaveData(ref GameData data)
    {
        int index = data.sapDataList.FindIndex(x => x.id == entityID.ID);
        if (index != -1) data.sapDataList[index].isEmpty = _isEmpty;
        else data.sapDataList.Add(new SapSaveData { id = entityID.ID, isEmpty = _isEmpty });
    }

    #endregion

    #region ISapLockable

    public Transform GetLockTransform() => transform;
    public bool      IsLockable()       => !_isEmpty;
    public float     GetLockPriority()  => 1f;
    public void      GiveSap()          => Collect();

    #endregion
}