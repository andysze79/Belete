using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void InRange();
    void OnHoverEnter();
    void OnHoverExit();
    void OnInteract();
    void OutOfRange();
}
