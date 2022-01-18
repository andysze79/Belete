using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class InteractableEvent : MonoBehaviour, IInteractable
{
    public UltEvent WhenInRange;
    public UltEvent WhenOutOfRange;
    public UltEvent WhenHoverEnter;
    public UltEvent WhenHoverExit;
    public UltEvent WhenInteract;

    public void InRange()
    {
        WhenInRange?.Invoke();
    }

    public void OnHoverEnter()
    {
        WhenHoverEnter?.Invoke();    
    }

    public void OnHoverExit()
    {
        WhenHoverExit?.Invoke();    
    }

    public void OnInteract()
    {
        WhenInteract?.Invoke();       
    }

    public void OutOfRange()
    {
        WhenOutOfRange?.Invoke();
    }
}
