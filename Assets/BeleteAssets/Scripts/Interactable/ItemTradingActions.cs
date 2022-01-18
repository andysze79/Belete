using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTradingActions : MonoBehaviour
{
    public string currentDecideItemName { get; set; }
    public void PlayerGetItem(string obj)
    {
        var itemObj = Belete.GameManager.Instance.m_ItemLibrary.GetItem(obj);
        Belete.GameManager.Instance.m_PlayerInventory.Add(itemObj);
        EventHandler.WhenReceiveItem?.Invoke(itemObj);
    }
    public bool isPlayerInventoryEmpty()
    {
        bool result = !(Belete.GameManager.Instance.m_PlayerInventory.Count > 0);
        
        return result;
    }
    public bool PlayerHasItem(string obj) {
        var item = Belete.GameManager.Instance.m_ItemLibrary.GetItem(obj);
        bool result = false;

        for (int i = 0; i < Belete.GameManager.Instance.m_PlayerInventory.Count; i++)
        {
            if (Belete.GameManager.Instance.m_PlayerInventory[i] == item)
                result = true;        
        }

        return  result;
    }
    public void PlayerGetItem(Item obj)
    {
        Belete.GameManager.Instance.m_PlayerInventory.Add(obj);
        EventHandler.WhenReceiveItem?.Invoke(obj);
    }
    public bool isUseItemMatchRequireItem(string requireItemName) {
        bool result = (currentDecideItemName == requireItemName);

        currentDecideItemName = null;

        return result;    
    }
    public void PlayerUseItem(Item obj)
    {
        Belete.GameManager.Instance.m_PlayerInventory.Remove(obj);
        EventHandler.WhenUseItem?.Invoke(obj);
    }
    public void GetItem(Item obj, List<Item> addToList) {
        addToList.Add(obj);
    }
    public void UseItem(Item obj, List<Item> removeFromList) {
        removeFromList.Remove(obj);
    }
    public void TradeItem(Item obj, List<Item> seller, List<Item> buyer) {
        seller.Remove(obj);
        buyer.Add(obj);
    }
}
