using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public string m_HorizontalName = "Horizontal";
    public string m_VerticalName = "Vertical";
    public string m_InteractAName = "Fire";
    public string m_InteractBName = "Fire";
    private static InputManager m_Instance;
    public static InputManager Instance {
        get {
            if (m_Instance != null)
                return m_Instance;
            else
            { 
                m_Instance = GameObject.FindObjectOfType<InputManager>();
                return m_Instance;
            }
        }
    }
    public float HorizontalValue { get; set; }
    public float VerticalValue { get; set; }
    public bool InteractAValue { get; set; }
    public bool InteractBValue { get; set; }

    public delegate void AxisEvent(float horizontalValue, float verticalValue);
    public AxisEvent GetAxisAValue { get; set; }

    public delegate void ButtonEvent(bool value);
    public ButtonEvent GetInteractAValue { get; set; }
    public ButtonEvent GetInteractBValue { get; set; }
    private void Update()
    {
        HorizontalValue = Input.GetAxisRaw(m_HorizontalName);
        VerticalValue = Input.GetAxisRaw(m_VerticalName);
        InteractAValue = Input.GetButtonUp(m_InteractAName);
        InteractBValue = Input.GetButtonUp(m_InteractBName);

        GetAxisAValue?.Invoke(HorizontalValue, VerticalValue);
        GetInteractAValue?.Invoke(InteractAValue);
        GetInteractBValue?.Invoke(InteractBValue);
    }
}
