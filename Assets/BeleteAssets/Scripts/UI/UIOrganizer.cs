using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOrganizer:MonoBehaviour
{
    public List<UILayerData> m_UIData = new List<UILayerData>();
    private UILayerData CurrentUILayerData { get; set; }

    public delegate void UIChangeDel(Belete.GameManager.GameState state);
    public UIChangeDel WhenUIChange;
    public bool isPuzzleUIOn;

    private void Awake()
    {
        CurrentUILayerData = m_UIData[0];

        EventHandler.WhenStartConversation += StartConversation;
        EventHandler.WhenEndConversation += EndConversation;
    }

    public GameObject GetLayerItem(UILayerData.OrganizerName organizerType, int layerItemIndex) {
        var target = GetUILayerData(organizerType);
        return target.LayerItems[layerItemIndex];
    }
    #region Activate specific UI Item ignore the related organizer
    public void ActivateOverlayUIItem(UILayerData.OrganizerName organizerType, int layerItemIndex) {
        var target = GetUILayerData(organizerType);

        target.LayerOrganizer.SetActive(true);
        target.LayerItems[layerItemIndex].SetActive(true);
    }
    public void DeactivateOverlayUIItem(UILayerData.OrganizerName organizerType, int layerItemIndex)
    {
        var target = GetUILayerData(organizerType);

        target.LayerItems[layerItemIndex].SetActive(false);

        // Only change related layer when all the ui item is unloaded        
        if (!isAllLayerItemsUnloaded(target)) return;

        target.LayerOrganizer.SetActive(false);
    }
    #endregion
    #region Activate specific UI Item
    public GameObject ActivateUILayerItem(UILayerData.OrganizerName organizerType, int layerItemIndex) {
        var target = GetUILayerData(organizerType);

        target.LayerOrganizer.SetActive(true);
        target.LayerItems[layerItemIndex].SetActive(true);
        
        WhenUIChange?.Invoke(target.WhenLoadedChangToState);

        SwitchOrganizer(target.WhenLoadedUnloadOrganizers, false);

        CurrentUILayerData = target;

        return target.LayerItems[layerItemIndex];
    }
    public GameObject DeactivateUILayerItem(UILayerData.OrganizerName organizerType, int layerItemIndex) {
        var target = GetUILayerData(organizerType);

        target.LayerItems[layerItemIndex].SetActive(false);

        // Only change related layer when all the ui item is unloaded      
        if (!isAllLayerItemsUnloaded(target)) return target.LayerItems[layerItemIndex]; 

        target.LayerOrganizer.SetActive(false);

        SwitchOrganizer(target.WhenUnloadedLoadOrganizers, true);

        return target.LayerItems[layerItemIndex];
    }
    #endregion
    #region Activate the whole UI Layer
    public void ActivateUILayer(UILayerData.OrganizerName organizerType)
    {
        var target = GetUILayerData(organizerType);

        target.LayerOrganizer.SetActive(true);

        SwitchOrganizer(target.WhenLoadedUnloadOrganizers, false);
    }
    public void DeactivateUILayer(UILayerData.OrganizerName organizerType) {
        var target = GetUILayerData(organizerType);
        
        target.LayerOrganizer.SetActive(false);

        SwitchOrganizer(target.WhenUnloadedLoadOrganizers, true);
    }
    #endregion 
    public void SwitchOrganizer(UILayerData.OrganizerName[] organizerName, bool value) {

        for (int i = 0; i < organizerName.Length; i++)
        {
            for (int j = 0; j < m_UIData.Count; j++)
            {
                if (m_UIData[j].OrganizerType == organizerName[i])
                {
                    m_UIData[j].LayerOrganizer.SetActive(value);

                    if (value)
                    { 
                        CurrentUILayerData = m_UIData[j];
                        WhenUIChange?.Invoke(m_UIData[i].WhenLoadedChangToState);
                    }
                }
            }
        }
    }
    private UILayerData GetUILayerData(UILayerData.OrganizerName OrganizerType)
    {
        for (int i = 0; i < m_UIData.Count; i++)
        {
            if (m_UIData[i].OrganizerType == OrganizerType)
                return m_UIData[i];
        }

        return null;
    }
    private bool isAllLayerItemsUnloaded(UILayerData targetLayer) {        
        for (int i = 0; i < targetLayer.LayerItems.Length; i++)
        {
            if (targetLayer.LayerItems[i].activeSelf)
                return false;
        }
        return true;
    }
    private void StartConversation() => DeactivateUILayer(UILayerData.OrganizerName.MainScreen);
    private void EndConversation() => ActivateUILayer(UILayerData.OrganizerName.MainScreen);
}
[System.Serializable]
public class UILayerData {
    public enum OrganizerName { 
        Notification, MainScreen, GameplayUI
    }
    public OrganizerName OrganizerType;
    public GameObject LayerOrganizer;
    public GameObject[] LayerItems;
    public OrganizerName[] WhenLoadedUnloadOrganizers;
    public OrganizerName[] WhenUnloadedLoadOrganizers;
    public Belete.GameManager.GameState WhenLoadedChangToState;
}
