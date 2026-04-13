using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotMenu : MenuButton
{

    [Header("Menu Navigation")] 
    [SerializeField] private MainMenuSave mainMenu;
    
    [Header("Menu Buttons")] 
    [SerializeField] private Button backButton;
    
    private SaveSlot[] saveSlots;

    private bool isLoadingGame = false;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }
    
    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {

        DisableMenuButtons();
        
        DataPersistenceManager.DataPM_instance.ChangeSelectedProfileID(saveSlot.GetProfileID());

        if (!isLoadingGame)
        {
            DataPersistenceManager.DataPM_instance.NewGame();
            DataPersistenceManager.DataPM_instance.SaveGame();
        }
        
        SceneManager.LoadSceneAsync("GymRoomCorentin");
    }

    public void OnBackClick()
    {
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        
        this.gameObject.SetActive(true);

        this.isLoadingGame = isLoadingGame;
        
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.DataPM_instance.GetAllProfileGameData();

        GameObject firstSelected = backButton.gameObject;
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);
            saveSlot.SetData(profileData);
            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
                if (firstSelected.Equals(backButton.gameObject))
                {
                    firstSelected = saveSlot.gameObject;
                }
            }
        }

        Button firstSelectedButton = firstSelected.GetComponent<Button>();
        this.SetFirstSelected(firstSelectedButton);

    }
    
    public void DeactivateMenu()
    { 
        this.gameObject.SetActive(false);
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }

        backButton.interactable = false;
    }
}
