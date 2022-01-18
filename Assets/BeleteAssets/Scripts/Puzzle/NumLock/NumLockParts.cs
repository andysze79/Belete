using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumLockParts : MonoBehaviour, IInteractable
{
    public NumLock numLock { get; set; }
    public void InRange()
    {
        //throw new System.NotImplementedException();
    }
    public void OnHoverEnter()
    {
        //throw new System.NotImplementedException();
    }

    public void OnHoverExit()
    {
        
    }

    public void OnInteract()
    {
        numLock.OnInteract(transform);
    }

    public void OutOfRange()
    {
        
    }
}
