using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LevelInteractablesController : MonoBehaviour
{
    public bool m_IgnoreInitialSetting;
    [System.Serializable]
    public struct InitialInteractableSettings {
        public InteractableSimple Interactable;
        public bool Active;
    }
    public List<InitialInteractableSettings> m_InitialSettings = new List<InitialInteractableSettings>();
    public List<LevelCondition> m_LevelStagingSettings = new List<LevelCondition>();

    private void Start()
    {
        if (m_IgnoreInitialSetting) return;

        for (int i = 0; i < m_InitialSettings.Count; i++)
        {
            if (m_InitialSettings[i].Active)
                m_InitialSettings[i].Interactable.TurnOnInteractable();
            else
                m_InitialSettings[i].Interactable.TurnOffInteractable();
        }
    }
    private void OnEnable()
    {
        for (int i = 0; i < m_LevelStagingSettings.Count; i++)
        {
            switch (m_LevelStagingSettings[i].CurrentCondition)
            {
                case LevelCondition.Condition.GainItem:
                    m_LevelStagingSettings[i].TriggerByGainObj.WhenGainItemDel += WhenMeetCondition;
                    break;
                case LevelCondition.Condition.GainPage:
                    m_LevelStagingSettings[i].TriggerByGainObj.WhenGainItemDel += WhenMeetCondition;
                    break;
                case LevelCondition.Condition.FixedPuzzle:
                    m_LevelStagingSettings[i].TriggerByPuzzleFix.WhenFixedPuzzle += WhenMeetCondition;
                    break;
                case LevelCondition.Condition.FixedUIPuzzle:
                    m_LevelStagingSettings[i].TriggerByUIPuzzleFix.WhenFixedUIPuzzle += WhenMeetFixedUIPuzzleCondition;
                    break;
                default:
                    break;
            }            
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < m_LevelStagingSettings.Count; i++)
        {
            switch (m_LevelStagingSettings[i].CurrentCondition)
            {
                case LevelCondition.Condition.GainItem:
                    m_LevelStagingSettings[i].TriggerByGainObj.WhenGainItemDel -= WhenMeetCondition;
                    break;
                case LevelCondition.Condition.GainPage:
                    m_LevelStagingSettings[i].TriggerByGainObj.WhenGainItemDel -= WhenMeetCondition;
                    break;
                case LevelCondition.Condition.FixedPuzzle:
                    m_LevelStagingSettings[i].TriggerByPuzzleFix.WhenFixedPuzzle -= WhenMeetCondition;
                    break;
                case LevelCondition.Condition.FixedUIPuzzle:
                    m_LevelStagingSettings[i].TriggerByUIPuzzleFix.WhenFixedUIPuzzle -= WhenMeetFixedUIPuzzleCondition;
                    break;
                default:
                    break;
            }
        }
    }
    private void WhenMeetCondition(ItemFunctions itemFunction) {
        int targetCondition = 0;
        
        for (int i = 0; i < m_LevelStagingSettings.Count; i++)
        {
            if (m_LevelStagingSettings[i].TriggerByGainObj == itemFunction && m_LevelStagingSettings[i].ItemFunctionsCurrentSettings == itemFunction.currentItemSettings)
                targetCondition = i;
        }

        SwitchInteractable(targetCondition);
        SwitchFlowchart(targetCondition);
    }
    private void WhenMeetCondition(InteractableSimple interactable)
    {
        print("Fixed " + interactable.name + " puzzle");
        int targetCondition = 0;

        for (int i = 0; i < m_LevelStagingSettings.Count; i++)
        {
            if (m_LevelStagingSettings[i].TriggerByPuzzleFix == interactable)
                targetCondition = i;
        }

        SwitchInteractable(targetCondition);
        SwitchFlowchart(targetCondition);
    }
    private void WhenMeetFixedUIPuzzleCondition(InteractableSimple interactable) {
        print("Fixed " + interactable.name + " UI puzzle");
        int targetCondition = 0;

        for (int i = 0; i < m_LevelStagingSettings.Count; i++)
        {
            if (m_LevelStagingSettings[i].TriggerByUIPuzzleFix == interactable)
            { 
                targetCondition = i;
                SwitchInteractable(targetCondition);
                SwitchFlowchart(targetCondition);
            }
        }
    }
    private void SwitchInteractable(int targetCondition) {
        for (int i = 0; i < m_LevelStagingSettings[targetCondition].EnableThese.Count; i++)
        {
            m_LevelStagingSettings[targetCondition].EnableThese[i].TurnOnInteractable();
        }

        for (int i = 0; i < m_LevelStagingSettings[targetCondition].DisableThese.Count; i++)
        {
            m_LevelStagingSettings[targetCondition].DisableThese[i].TurnOffInteractable();
        }
    }
    private void SwitchFlowchart(int targetCondition) {
        for (int i = 0; i < m_LevelStagingSettings[targetCondition].SetFlowchart.Count; i++)
        {
            m_LevelStagingSettings[targetCondition].SetFlowchart[i].GetComponent<Fungus.Flowchart>().SetBooleanVariable(
                m_LevelStagingSettings[targetCondition].BoolName,
                m_LevelStagingSettings[targetCondition].BoolValue
                );

            if (m_LevelStagingSettings[targetCondition].StartFlowChartIndex >= 0)
            {
                m_LevelStagingSettings[targetCondition].SetFlowchart[i].ChangeFlowChartStartIndex(
                    m_LevelStagingSettings[targetCondition].StartFlowChartIndex);
            }
        }
    }
}

[System.Serializable]
public class LevelCondition {
    public enum Condition { GainItem, GainPage, FixedPuzzle, FixedUIPuzzle}
    public Condition CurrentCondition;
    [ShowIf("CurrentCondition", Condition.GainItem)] public ItemFunctions TriggerByGainObj;
    [ShowIf("CurrentCondition", Condition.GainItem)] public int ItemFunctionsCurrentSettings = 0;
    [ShowIf("CurrentCondition", Condition.FixedPuzzle)] public InteractableSimple TriggerByPuzzleFix;
    [ShowIf("CurrentCondition", Condition.FixedUIPuzzle)] public InteractableSimple TriggerByUIPuzzleFix;
    [FoldoutGroup("Switch Interactable Settings")] public List<InteractableSimple> EnableThese;
    [FoldoutGroup("Switch Interactable Settings")] public List<InteractableSimple> DisableThese;
    [FoldoutGroup("Change Flow Chart Settings")] public string BoolName;
    [FoldoutGroup("Change Flow Chart Settings")] public bool BoolValue;
    [FoldoutGroup("Change Flow Chart Settings")] public int StartFlowChartIndex = -1;
    [FoldoutGroup("Change Flow Chart Settings")] public List<InteractableSimple> SetFlowchart;    
}
