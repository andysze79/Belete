using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestFlowchartStory : MonoBehaviour
{
    public void SendRequest(string id) {
        Belete.GameManager.Instance.m_FlowchartExecutor.ExecuteBlock(id);
    }
}
