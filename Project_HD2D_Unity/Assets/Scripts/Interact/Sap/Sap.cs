using System;
using UnityEngine;

public class Sap : MonoBehaviour, ISapLockable, IDataPersistence
{
    [SerializeField] private EntityID entityID;
    [SerializeField] private GameObject vfxSapPresent;
    private bool _isEmpty;
    

    #region Save

    public void LoadData(GameData data)
    {
        SapSaveData myData = data.sapDataList.Find(x => x.id == entityID.ID);
        
        if (myData != null)
        {
            this._isEmpty = myData.isEmpty;

            vfxSapPresent.SetActive(!myData.isEmpty);
        }
    }

    public void SaveData(ref GameData data)
    {
        int index = data.sapDataList.FindIndex(x => x.id == entityID.ID);
        if (index != -1) data.sapDataList[index].isEmpty = this._isEmpty;
        else data.sapDataList.Add(new SapSaveData { id = entityID.ID, isEmpty = this._isEmpty });
    }

    #endregion
    
    
    #region ISapLockable
    public Transform GetLockTransform() => transform;

    public bool IsLockable() => !_isEmpty;

    public float GetLockPriority() => 1f;

    public void GiveSap()
    {
        _isEmpty = true;
        vfxSapPresent.SetActive(false);
        UiEvents.TriggerShowPopup();
    }
    #endregion
}