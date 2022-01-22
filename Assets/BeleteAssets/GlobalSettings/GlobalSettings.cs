using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Belete/ GlobalSettings")]
public class GlobalSettings: ScriptableObject
{

    [SerializeField] private float m_InteractableFlowchartCDTime = .5f;
    [SerializeField] private float m_HighLightGlow = 5f;

    public float InteractableFlowchartCDTime { get { return m_InteractableFlowchartCDTime; } }
    public float HighLightGlow { get { return m_HighLightGlow; } }
}
