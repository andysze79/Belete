using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UseItemMenu : MonoBehaviour
{
    public GameObject m_MenuItem;
    public ScrollRect m_ScrollRect;
    public Image m_ItemImage;
    public TextMeshProUGUI m_ItemName;
    public TextMeshProUGUI m_ItemDescription;
    [SerializeField] private List<GameObject> m_CurrentContentUI = new List<GameObject>();
    
    bool choosedItem;

    private void Awake()
    {
        var contents = m_ScrollRect.GetComponentsInChildren<UseItemMenuContent>();

        foreach (var item in contents)
        {
            m_CurrentContentUI.Add(item.gameObject);
        }
    }
    private void OnEnable()
    {
        choosedItem = false;

        TurnOnOffItemInfo(false);

        UpdateContents();
    }
    private void OnDisable()
    {
        for (int i = 0; i < m_CurrentContentUI.Count; i++)
        {
            TurnOffContentUI(m_CurrentContentUI[i]);
        }
        
        if(!choosedItem)
            FailToDecideItem();
    }
    private void UpdateContents()
    {
        var items = Belete.GameManager.Instance.m_PlayerInventory;
        UseItemMenuContent menuContent;

        // turn off all the content UI
        for (int i = 0; i < m_CurrentContentUI.Count; i++)
        {
            TurnOffContentUI(m_CurrentContentUI[i]);
        }

        // adjust content UI amount to fit the items amount that player has
        if (items.Count != m_CurrentContentUI.Count) {
            // need more content ui
            if (items.Count > m_CurrentContentUI.Count) {
                var difference = items.Count - m_CurrentContentUI.Count;
                for (int i = 0; i < difference; i++)
                {                    
                    var clone = Instantiate(m_MenuItem, m_ScrollRect.content);
                    m_CurrentContentUI.Add(clone);
                    clone.SetActive(false);
                }
            }            
        }
        // only turn on the require UI and update it's value
        for (int i = 0; i < items.Count; i++)
        {            
            UpdateContentInfo(m_CurrentContentUI[i], items[i]);
            TurnOnContentUI(m_CurrentContentUI[i]);            
        }
    }
    private void UpdateContentInfo(GameObject contentUI, Item itemInfo)
    {
        var menuContent = contentUI.GetComponentInChildren<UseItemMenuContent>();

        menuContent.item = itemInfo;

        contentUI.GetComponentInChildren<TextMeshProUGUI>().text = itemInfo.UIName;        
    }
    private void TurnOnContentUI(GameObject contentUI) {
        var menuContent = contentUI.GetComponentInChildren<UseItemMenuContent>();
        menuContent.WhenPointerEnter += UpdateItemDescription;
        menuContent.WhenPointerExit += CloseItemDescription;
        
        if(Belete.GameManager.Instance.m_CurrentGameState == Belete.GameManager.GameState.Conversation)
            menuContent.WhenPointerClick += DecideItem;

        contentUI.SetActive(true);
    }
    private void TurnOffContentUI(GameObject contentUI)
    {
        var menuContent = contentUI.GetComponentInChildren<UseItemMenuContent>();
        menuContent.WhenPointerEnter -= UpdateItemDescription;
        menuContent.WhenPointerExit -= CloseItemDescription;

        if (Belete.GameManager.Instance.m_CurrentGameState == Belete.GameManager.GameState.Conversation)
            menuContent.WhenPointerClick -= DecideItem;

        contentUI.SetActive(false);
    }
   
    private void UpdateItemDescription(PointerEventData eventData) {
        if (eventData.pointerEnter.TryGetComponent(out UseItemMenuContent content)) {
            m_ItemName.text = content.item.UIName;
            m_ItemDescription.text = content.item.Description;
            m_ItemImage.sprite = content.item.Image;
            
            TurnOnOffItemInfo(true);
        }
    }
    private void TurnOnOffItemInfo(bool value) {
        m_ItemImage.enabled = value;
        m_ItemName.enabled = value;
        m_ItemDescription.enabled = value;
    }
    private void CloseItemDescription(PointerEventData eventData)
    {
        TurnOnOffItemInfo(false);
    }
    private void DecideItem(PointerEventData eventData)
    {
        if (eventData.pointerPress.TryGetComponent(out UseItemMenuContent content))
        {            
            choosedItem = true;
            Belete.GameManager.Instance.ItemTradingActions.currentDecideItemName = content.item.Name;
            EventHandler.WhenDecidedWhichItemToUse?.Invoke();
            Belete.GameManager.Instance.m_UIManager.SwitchUseItemUI(false);
        }
    }
    private void FailToDecideItem()
    {
        Belete.GameManager.Instance.ItemTradingActions.currentDecideItemName = "";
        EventHandler.WhenDecidedWhichItemToUse?.Invoke();        
    }
}
