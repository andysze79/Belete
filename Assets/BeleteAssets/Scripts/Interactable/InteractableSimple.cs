using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using Sirenix.OdinInspector;

public class InteractableSimple : MonoBehaviour, IInteractable
{
    public enum InteractableType { SceneObj, UIObj}
    public string[] m_FlowChartStart = { "Start" };
    public InteractableType m_InteractableType = InteractableType.SceneObj;
    private int currentFlowChartStartIndex;
    [ReadOnly]public bool m_IgnoreRange = false;
    [ReadOnly]public Flowchart m_FlowChart;
    [ReadOnly]public HighlightPlus.HighlightEffect m_HighLightEffect;
    [ReadOnly]public bool active;
    [ReadOnly] [SerializeField]private float CDTime = 1.5f;
    [ReadOnly] [SerializeField]private float HighLightGlow = 5f;
    public delegate void InteractableDel();
    public InteractableDel WhenInteract; 
    public delegate void InteractableSimpleDel(InteractableSimple interactable);
    public InteractableSimpleDel WhenFixedPuzzle;
    public InteractableSimpleDel WhenFixedUIPuzzle;
    public List<IInteractableSettingsUpdate> listenersList = new List<IInteractableSettingsUpdate>(); 
    Coroutine CDprocess;

    private void Awake()
    {
        var listenerArray = GetComponentsInChildren<IInteractableSettingsUpdate>();
        listenersList = new List<IInteractableSettingsUpdate>(listenerArray);
    }

    private void OnEnable()
    {
        m_FlowChart = GetComponent<Flowchart>();
        m_HighLightEffect= GetComponentInChildren<HighlightPlus.HighlightEffect>();
                
        m_HighLightEffect.highlighted = false;

        switch (m_InteractableType)
        {
            case InteractableType.SceneObj:
                m_IgnoreRange = false; 
                m_FlowChart.enabled = false;
                break;
            case InteractableType.UIObj:
                m_IgnoreRange = true; 
                m_FlowChart.enabled = true;
                break;
            default:
                break;
        }

        active = m_IgnoreRange;
    }
    public void ChangeFlowChartStartIndex(int index) {
        currentFlowChartStartIndex = index;
        
        for (int i = 0; i < listenersList.Count; i++)
        {
            listenersList[i].OnNotify(index);
        }
    }
    public void InteractWithUIObj() {
        InRange();
        OnHoverEnter();        
        Belete.GameManager.Instance.ChangeInteractableObj(this);
        OnInteract();
    }
    public void TurnOffInteractable() {
        OnHoverExit();
        OutOfRange();
        gameObject.layer = LayerMask.NameToLayer("Default");

        EventHandler.WhenSwitchInteractableSimple?.Invoke();
    }
    public void TurnOnInteractable()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        EventHandler.WhenSwitchInteractableSimple?.Invoke();
    }
    public void InRange()
    {
        m_FlowChart.enabled = true;
        m_HighLightEffect.highlighted = true;
        active = true;
    }
    public void OnHoverEnter()
    {
        if (!active) return;
        if(m_InteractableType == InteractableType.UIObj) m_HighLightEffect.highlighted = true;

        m_HighLightEffect.glow = HighLightGlow;
        
        var mouseInput = Input.mousePosition;
        Belete.GameManager.Instance.m_UIManager.WhenHoverEnterInteractableObj(this, mouseInput);
    }
    public void OnHoverExit()
    {
        if(m_InteractableType == InteractableType.UIObj) m_HighLightEffect.highlighted = false;

        if(m_HighLightEffect)
            m_HighLightEffect.glow = .01f;

        Belete.GameManager.Instance.m_UIManager.WhenHoverExitInteractableObj();
    }
    public void OnInteract()
    {
        if (!m_FlowChart.enabled) return;
        if(m_FlowChart.HasExecutingBlocks()) return;

        m_FlowChart.ExecuteBlock(m_FlowChartStart[currentFlowChartStartIndex]);
        WhenInteract?.Invoke();

        EventHandler.WhenStartConversation?.Invoke();
        print("Start Conversation");
    }

    public void OutOfRange()
    {
        if (CDprocess != null)
            StopCoroutine(CDprocess);

        m_FlowChart.enabled = false;
        m_HighLightEffect.highlighted = false;
        active = false;

        m_HighLightEffect.glow = .01f;
    }
    public void Finished() {
        if (CDprocess != null)
            StopCoroutine(CDprocess);
        CDprocess = StartCoroutine(CoolDown());

        EventHandler.WhenEndConversation?.Invoke();

        m_HighLightEffect.glow = .01f;

        print("End Conversation");
    }
    public void FinishedAndShowPuzzle()
    {
        if (CDprocess != null)
            StopCoroutine(CDprocess);
        CDprocess = StartCoroutine(CoolDown());

        EventHandler.WhenEndPuzzleConversation?.Invoke();

        m_HighLightEffect.glow = .01f;

        print("End Puzzle Conversation");
    }
    public void FinishedInvestigatePuzzleElements()
    {
        if (CDprocess != null)
            StopCoroutine(CDprocess);
        CDprocess = StartCoroutine(CoolDown());

        EventHandler.WhenEndPuzzleInvestigation?.Invoke();        

        m_HighLightEffect.glow = .01f;

        print("End Puzzle Investigation");
    }
    public void FixedPuzzle() {
        WhenFixedPuzzle?.Invoke(this);
    }
    public void FixedUIPuzzle(PuzzleSubject puzzleSubject)
    {
        WhenFixedUIPuzzle?.Invoke(this);
    }
    private IEnumerator CoolDown()
    {
        m_FlowChart.enabled = false;
        yield return new WaitForSeconds(CDTime);
        m_FlowChart.enabled = true;        
    }
}
