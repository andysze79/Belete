using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageInteractableControl : MonoBehaviour
{
    private UnityEngine.UI.Button[] Buttons;
    private void Awake()
    {
        Buttons = GetComponentsInChildren<UnityEngine.UI.Button>();
    }
    private void OnEnable()
    {
        EventHandler.WhenShowingAquirePage += InverterForSwitchButton;
    }
    private void OnDisable()
    {
        EventHandler.WhenShowingAquirePage -= InverterForSwitchButton;
    }
    private void InverterForSwitchButton(bool value) {
        SwitchButtons(!value);
    }
    public void SwitchButtons(bool value) {
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].interactable = value;
        }
    }
}
