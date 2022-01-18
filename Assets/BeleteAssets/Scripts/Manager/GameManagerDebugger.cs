using Belete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerDebugger : MonoBehaviour,Belete.IGameStateEvent
{
    string myLog;
    Queue myLogQueue = new Queue();

    private void OnEnable()
    {
        Belete.GameManager.Instance.AddNotifyMember(this);
        Application.logMessageReceived += HandleLog;
    }
    private void OnDisable()
    {
        Belete.GameManager.Instance.RemoveNotifyMember(this);        
        Application.logMessageReceived -= HandleLog;
    }

    public void OnNotifyGameStateChanged(GameManager.GameState state)
    {
        Debug.Log(state);
    }
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLog = logString;
        string newString = "\n [" + type + "] : " + myLog;
        myLogQueue.Enqueue(newString);
        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            myLogQueue.Enqueue(newString);
        }
        myLog = string.Empty;
        foreach (string mylog in myLogQueue)
        {
            myLog += mylog;
        }
    }

    void OnGUI()
    {
        GUILayout.Label(myLog);
    }
}
