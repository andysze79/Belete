using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class FlowchartExecutor : MonoBehaviour
{
    public Flowchart m_StoryExecutor;

    public void ExecuteBlock(string id) {
        m_StoryExecutor.ExecuteBlock(id);
    }
}
