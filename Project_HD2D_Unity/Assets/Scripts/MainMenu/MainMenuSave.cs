using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSave : MenuButton
{

    [Header("Menu Navigation")] 
    [SerializeField] private SaveSlotMenu saveSlotMenu;
    
    [Header("Menu buttons")] 
    [SerializeField] private List<Button> ButtonList;
    [SerializeField] private Button ContinueButton;
    [SerializeField] private Button LoadGameButton;

    private void Start()
    {
        if (!DataPersistenceManager.DataPM_instance.HasGameData())
        {
            ContinueButton.interactable = false;
            LoadGameButton.interactable = false;
        }
    }


    public void OnNewGameClicked()
    {
        saveSlotMenu.ActivateMenu(false);
        this.DeactivateMenu();
    }
    
    public void OnLoadGameClicked()
    {
        saveSlotMenu.ActivateMenu(true);
        this.DeactivateMenu();
    }

    public void OnContinueClicked()
    {
        
        DisableMenuButton();
        
        DataPersistenceManager.DataPM_instance.SaveGame();
        
        SceneManager.LoadSceneAsync("GymRoomCorentin");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
        Debug.Log("Exit the game !");
    }

    private void DisableMenuButton()
    {
        foreach (var buttons in ButtonList)
        {
            buttons.interactable = false;
        }
    }
    
    public void ActivateMenu()
    {
        this.gameObject.SetActive(true);
    }
    
    public void DeactivateMenu()
    { 
        this.gameObject.SetActive(false);
    }
    
    
}
