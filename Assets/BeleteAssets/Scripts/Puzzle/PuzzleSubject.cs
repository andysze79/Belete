using System;
using UnityEngine;

public abstract class PuzzleSubject : MonoBehaviour
{
    public string m_PuzzleID;
    public static event Action<PuzzleSubject> RequestorEvent;

    public void NotifyRequestorEvent() {
        RequestorEvent?.Invoke(this);
    }
    protected virtual void OnDisable()
    {
        RequestorEvent = null;
    }
}
