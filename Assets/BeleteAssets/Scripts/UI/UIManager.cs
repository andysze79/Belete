using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using Belete;

public class UIManager : MonoBehaviour, Belete.IGameStateEvent
{
    public float m_InteractableUIYoffset;
    public RectTransform m_InteractableUI;
    public TextMeshProUGUI m_InteractableText;

    [FoldoutGroup("Get Item UI")] public GameObject m_GetItemUI;
    [FoldoutGroup("Get Item UI")] public Image m_GetItemImage;
    [FoldoutGroup("Get Item UI")] public TextMeshProUGUI m_GetItemText;
    [FoldoutGroup("Get Item UI")] public string m_GetItemTextFormat;
    [FoldoutGroup("Get Item UI")] public float m_GetItemUIActiveTime;

    [FoldoutGroup("Get Page UI")] public GameObject m_GetPageUI;
    [FoldoutGroup("Get Page UI")] public Image m_GetPageImage;
    [FoldoutGroup("Get Page UI")] public TextMeshProUGUI m_GetPageText;
    [FoldoutGroup("Get Page UI")] public string m_GetPageTextFormat;
    [FoldoutGroup("Get Page UI")] public float m_GetPageUIActiveTime;

    private GameObject currentGetPageImageClone;

    public NoteBook m_NotebookUI;

    [Header("Reference Settings")]
    public UIPageLibrary m_UIPageLibrary;
    public UIOrganizer m_UIOrganizer;
    public int m_NotebookUIIndex;
    public int m_UseItemUIIndex;
    [Header("Debug")]
    [SerializeField][ReadOnly]private GameObject CurrentPuzzleUIPage;

    public delegate void UIDel(GameObject target);
    public UIDel WhenActivateUILayer; 
    public delegate void UIChangeDel(Belete.GameManager.GameState state);
    public UIChangeDel WhenUIChange;

    private void Awake()
    {
        TryGetComponent<UIOrganizer>(out m_UIOrganizer);
        
    }
    private void OnEnable()
    {
        EventHandler.WhenReceiveItem += GetItem;
        EventHandler.WhenReceiveNotebookPage += GetNotebookPage;
        GameManager.Instance.AddNotifyMember(this);
    }
    private void OnDisable()
    {
        EventHandler.WhenReceiveItem -= GetItem;        
        EventHandler.WhenReceiveNotebookPage -= GetNotebookPage;
        GameManager.Instance.RemoveNotifyMember(this);
    }
    
    public void WhenHoverEnterInteractableObj(InteractableSimple obj, Vector3 mouseInput)
    {
        // Text follow cursor
        m_InteractableUI.position = mouseInput + new Vector3(0, m_InteractableUIYoffset, 0);
        m_InteractableText.text = obj.name;
        m_InteractableUI.gameObject.SetActive(true);
    }
    public void WhenHoverExitInteractableObj()
    {
        m_InteractableUI.gameObject.SetActive(false);
    }
    public void GetItem(Item item) {
        m_GetItemImage.sprite = item.Image;
        m_GetItemText.text = (m_GetItemTextFormat + " " + item.UIName);
        m_GetItemUI.SetActive(true);
        StartCoroutine(DelayTurnOffUI(m_GetItemUI, m_GetItemUIActiveTime));
    }
    public void GetNotebookPage(GameObject page) {
        m_GetPageImage.gameObject.SetActive(false);       

        ShowGainPageNotification(page);

        StartCoroutine(GetPageSequence(page));
    }
    private void ShowGainPageNotification(GameObject page)
    {
        var pageInfo = page.GetComponent<Page>();
        // Page image positioning
        if (currentGetPageImageClone != null) Destroy(currentGetPageImageClone);
        currentGetPageImageClone = Instantiate(page, m_GetPageImage.transform.parent);
        currentGetPageImageClone.transform.rotation = m_GetPageImage.transform.rotation;
        currentGetPageImageClone.transform.localScale = m_GetPageImage.transform.localScale;
        TurnOffPageButtonsInteractable(currentGetPageImageClone);

        m_GetPageText.text = (m_GetPageTextFormat + " " + pageInfo.m_PageUIName);
        m_GetPageUI.SetActive(true);
    }
    private void TurnOffPageButtonsInteractable(GameObject obj) {
        var buttons = obj.GetComponentsInChildren<UnityEngine.UI.Button>();
        
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
    }
    private IEnumerator GetPageSequence(GameObject page) {
        yield return StartCoroutine(DelayTurnOffUI(m_GetPageUI, m_GetPageUIActiveTime));

        m_NotebookUI.AddPage(page);
        //m_NotebookUI.gameObject.SetActive(true);
        m_UIOrganizer.ActivateUILayerItem(UILayerData.OrganizerName.GameplayUI, m_NotebookUIIndex);
        m_NotebookUI.ShowPage(page.GetComponent<Page>().m_ListIndex);
    }
    public void SwitchUseItemUI(bool value) {
        if (value)
            m_UIOrganizer.ActivateOverlayUIItem(UILayerData.OrganizerName.GameplayUI, m_UseItemUIIndex);
        else
            m_UIOrganizer.DeactivateOverlayUIItem(UILayerData.OrganizerName.GameplayUI, m_UseItemUIIndex);

    }
    void IGameStateEvent.OnNotifyGameStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Adventure:

                break;
            case GameManager.GameState.Conversation:

                break;
            default:
                break;
        }
    }
    private IEnumerator DelayTurnOffUI(GameObject obj, float duration) {
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
    }

    #region UI Page Manage
    public GameObject RequestUILayer(UILayerData.OrganizerName OrganizerType, int layerItemIndex) {
        return m_UIOrganizer.ActivateUILayerItem(OrganizerType, layerItemIndex);        
    }
    public GameObject CloseUILayer(UILayerData.OrganizerName OrganizerType, int layerItemIndex) {
        return m_UIOrganizer.DeactivateUILayerItem(OrganizerType, layerItemIndex);        
    }
    public GameObject RequestUIPage(string id) {
        UIPageData pageData = m_UIPageLibrary.GetPageData(id);

        if (pageData.ClonePage == null)
        {
            var clone = Instantiate(pageData.UIPage, transform.parent);
            pageData.ClonePage = clone;

            if (clone.TryGetComponent<Canvas>(out Canvas canvas) && canvas.renderMode == RenderMode.ScreenSpaceOverlay)            
                canvas.worldCamera = Belete.GameManager.Instance.MainCam;
            
        }

        pageData.ClonePage.SetActive(true);

        WhenActivateUILayer?.Invoke(pageData.ClonePage);

        WhenUIChange?.Invoke(pageData.ChangeToState);
        
        CurrentPuzzleUIPage = pageData.ClonePage;
        m_UIOrganizer.isPuzzleUIOn = true;

        return pageData.ClonePage;
    }
    public void CloseUIPage(string id) {
        m_UIPageLibrary.GetPageData(id).ClonePage.SetActive(false);

        CurrentPuzzleUIPage = null;
        m_UIOrganizer.isPuzzleUIOn = false;

        // After puzzle page closed, reset the game state and UI state
        WhenUIChange?.Invoke(GameManager.GameState.Adventure);
        m_UIOrganizer.ActivateUILayer(UILayerData.OrganizerName.MainScreen);
    }
    #endregion

}



