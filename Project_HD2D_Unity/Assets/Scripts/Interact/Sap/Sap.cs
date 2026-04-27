using System;
using UnityEngine;

public class Sap : MonoBehaviour, IDataPersistence
{
    [SerializeField] private ParticleSystem particleFeedback;
    [SerializeField] private SpriteRenderer prayerIcon;
    [SerializeField] private EntityID entityID;
    private bool _isEmpty;

    #region Save

    public void LoadData(GameData data)
    {
        SapSaveData myData = data.sapDataList.Find(x => x.id == entityID.ID);
        
        if (myData != null)
        {
            this._isEmpty = myData.isEmpty;
        }
    }

    public void SaveData(ref GameData data)
    {
        int index = data.sapDataList.FindIndex(x => x.id == entityID.ID);
        if (index != -1) data.sapDataList[index].isEmpty = this._isEmpty;
        else data.sapDataList.Add(new SapSaveData { id = entityID.ID, isEmpty = this._isEmpty });
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!_isEmpty) GiveSap();
    }

    #region ISapLockable
    //public Transform GetLockTransform() => transform;

    //public bool IsLockable() => !_isEmpty;

    //public float GetLockPriority() => 1f;

    public void GiveSap()
    {
        Debug.Log("SAP GIVE SAP");
        particleFeedback.Stop();
        prayerIcon.gameObject.SetActive(false);
        _isEmpty = true;
        
        var player = FindFirstObjectByType<PlayerManager>();
        player.Context.PlayerData.AddSap();
        UiEvents.TriggerSapChanged(player.Context.PlayerData.Sap);
    }
    #endregion
}