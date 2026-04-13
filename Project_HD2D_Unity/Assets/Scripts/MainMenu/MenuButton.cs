using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [Header("First Selected button")]
    [SerializeField] private Button firstSelected;

    protected void OnEnable()
    {
        SetFirstSelected(firstSelected);
    }

    public void SetFirstSelected(Button button)
    {
        button.Select();
    }
}
