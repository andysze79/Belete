using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRequestor : MonoBehaviour
{
    public string[] m_RequestId;
    public UltEvents.UltEvent WhenPuzzleCleared;    
    public void RequestThisUI(UILayerData.OrganizerName OrganizerType, int layerItemIndex) {
        Belete.GameManager.Instance.m_UIManager.RequestUILayer(OrganizerType, layerItemIndex);        
    }
    public void CloseThisUI(UILayerData.OrganizerName OrganizerType, int layerItemIndex)
    {
        Belete.GameManager.Instance.m_UIManager.CloseUILayer(OrganizerType, layerItemIndex);     
    }
    public void RequestThisPuzzluUI(int index)
    {
        Belete.GameManager.Instance.m_UIManager.RequestUIPage(m_RequestId[index]);

        PuzzleSubject.RequestorEvent += PuzzluClear;

        TryGetComponent<InteractableSimple>(out InteractableSimple interactableSimple);
        PuzzleSubject.RequestorEvent += interactableSimple.FixedUIPuzzle;
    }
    public void CloseThisUI(int index)
    {
        Belete.GameManager.Instance.m_UIManager.CloseUIPage(m_RequestId[index]); 
    }

    private void PuzzluClear(PuzzleSubject t)
    {
        if (!CheckPuzzleID(t.m_PuzzleID)) return;

        print("puzzle clear");
        PuzzleSubject.RequestorEvent -= PuzzluClear;

        TryGetComponent<InteractableSimple>(out InteractableSimple interactableSimple);
        PuzzleSubject.RequestorEvent -= interactableSimple.FixedUIPuzzle;

        WhenPuzzleCleared?.Invoke();
    }
    private bool CheckPuzzleID(string id) {
        for (int i = 0; i < m_RequestId.Length; i++)
        {
            if (m_RequestId[i] == id) return true;
        }
        return false;
    }
}
