using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Fungus;
using UltEvents;
/// <summary>
/// For flow chart interaction
/// </summary>
public class ItemFunctions : MonoBehaviour, IInteractableSettingsUpdate
{
    [System.Serializable]
    public class ItemSettings {
        public int FlowChartIndex;
        public string GainItemName;
        public string GainNotebookPageName;
        public string RequireItemName;
        public string InventoryHasItemName = "InventoryHasItem";
        public string ReceiveRequireItemFlowChartBoolName;
    }

    public List<ItemSettings> m_ItemSettings = new List<ItemSettings>();

    [FoldoutGroup("Current Item Settings")] [ReadOnly] public string m_GainItemName;
    [FoldoutGroup("Current Item Settings")] [ReadOnly] public string m_GainNotebookPageName;
    [FoldoutGroup("Current Item Settings")] [ReadOnly] public string m_RequireItemName;
    [FoldoutGroup("Current Item Settings")] [ReadOnly] public string m_InventoryHasItemName = "InventoryHasItem";
    [FoldoutGroup("Current Item Settings")] [ReadOnly] public string m_ReceiveRequireItemFlowChartBoolName;
    public string m_DecidedName = "Decided";
    public UltEvent WhenGainItem;
    public delegate void ItemDel(ItemFunctions itemFunction);
    public ItemDel WhenGainItemDel;

    private Flowchart flowChart;
    public int currentItemSettings {get; private set; }
    private void Awake()
    {
        flowChart = GetComponent<Flowchart>();
        ChangeItemSettings(currentItemSettings);
    }
    private void OnEnable()
    {
        //EventHandler.WhenDecidedWhichItemToUse += DecidedWhichItemToUse;
    }
    private void OnDisable()
    {
        //EventHandler.WhenDecidedWhichItemToUse -= DecidedWhichItemToUse;
    }
    public void ChangeItemSettings(int index) {
        if (index >= m_ItemSettings.Count) return;

        currentItemSettings = index;
        m_GainItemName = m_ItemSettings[currentItemSettings].GainItemName;
        m_GainNotebookPageName = m_ItemSettings[currentItemSettings].GainNotebookPageName;
        m_RequireItemName = m_ItemSettings[currentItemSettings].RequireItemName;
        m_InventoryHasItemName = m_ItemSettings[currentItemSettings].InventoryHasItemName;
        m_ReceiveRequireItemFlowChartBoolName = m_ItemSettings[currentItemSettings].ReceiveRequireItemFlowChartBoolName;
    }
    public void GainOneItem()
    {
        if (isInventoryHasItem(m_GainItemName)) return;

        Belete.GameManager.Instance.ItemTradingActions.PlayerGetItem(m_GainItemName);

        WhenGainItem?.Invoke();
        WhenGainItemDel?.Invoke(this);
    }
    public void GainItem() {
        Belete.GameManager.Instance.ItemTradingActions.PlayerGetItem(m_GainItemName);
        
        WhenGainItem?.Invoke();
        WhenGainItemDel?.Invoke(this);
    }
    public void GainItem(string name)
    {
        Belete.GameManager.Instance.ItemTradingActions.PlayerGetItem(name);
        
        WhenGainItem?.Invoke();
        WhenGainItemDel?.Invoke(this);
    }
    public void GainNotebookPage() 
    {
        EventHandler.WhenReceiveNotebookPage?.Invoke(Belete.GameManager.Instance.m_NotebookPageLibrary.GetPage(m_GainNotebookPageName).gameObject);
        WhenGainItemDel?.Invoke(this);
    }
    private bool isInventoryHasItem(string name) {
        bool hasItem = Belete.GameManager.Instance.ItemTradingActions.PlayerHasItem(name);
                
        return hasItem;
    }
    private bool isInventoryHasRequireItem() {
        bool hasItem = Belete.GameManager.Instance.ItemTradingActions.PlayerHasItem(m_RequireItemName);
                
        return hasItem;
    }
    public void CheckInventoryHasItem(string flowChartBoolName)
    {
        bool hasItem = Belete.GameManager.Instance.ItemTradingActions.PlayerHasItem(m_RequireItemName);
        print(flowChartBoolName + hasItem);
        flowChart.SetBooleanVariable(flowChartBoolName, hasItem);
    }
    public void CheckIsInventoryEmpty()
    {
        bool hasItem = !Belete.GameManager.Instance.ItemTradingActions.isPlayerInventoryEmpty();

        flowChart.SetBooleanVariable(m_InventoryHasItemName, hasItem);
    }
    public void ChooseItemToUse() {
        Belete.GameManager.Instance.CheckUseItem();
        EventHandler.WhenDecidedWhichItemToUse += DecidedWhichItemToUse;
    }
    private void DecidedWhichItemToUse() {
        if (!flowChart) return;

        flowChart.SetBooleanVariable(m_DecidedName, true);
        EventHandler.WhenDecidedWhichItemToUse -= DecidedWhichItemToUse;
    }
    public void CheckReceiveItem() {
        /// if not receiving any item
        //if(Belete.GameManager.Instance.ItemTradingActions.currentDecideItemName == "") 

        if (Belete.GameManager.Instance.ItemTradingActions.isUseItemMatchRequireItem(m_RequireItemName))
        {
            flowChart.SetBooleanVariable(m_ReceiveRequireItemFlowChartBoolName, true);
        }
        else { 
            flowChart.SetBooleanVariable(m_ReceiveRequireItemFlowChartBoolName, false);            
        }

        flowChart.SetBooleanVariable(m_DecidedName, false);
    }
    public void UseRequireItem()
    {
        Belete.GameManager.Instance.ItemTradingActions.PlayerUseItem(Belete.GameManager.Instance.m_ItemLibrary.GetItem(m_RequireItemName));
    }
    public void UseItem(string name) {
        Belete.GameManager.Instance.ItemTradingActions.PlayerUseItem(Belete.GameManager.Instance.m_ItemLibrary.GetItem(name));
    }
    public void TurnOnOffUseItemMenu(bool value) {
        Belete.GameManager.Instance.m_UIManager.SwitchUseItemUI(value);
    }

    public void OnNotify(int flowChartStartIndex)
    {        
        for (int i = 0; i < m_ItemSettings.Count; i++)
        {
            if (m_ItemSettings[i].FlowChartIndex == flowChartStartIndex) { 
                ChangeItemSettings(i);            
            }
        }
    }
}
