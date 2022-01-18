using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonEvent : MonoBehaviour
{
    public UltEvents.UltEvent WhenClick;
    private Button Button { get; set; }
    private void Awake()
    {        
        Button = GetComponent<Button>();
        Button.onClick.AddListener(CallOnClick);
    }
    private void CallOnClick() {        
        WhenClick?.Invoke();
    }

}
