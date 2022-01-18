using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class NoteBook : MonoBehaviour
{
    public Transform m_PageContainer;
    public List<Image> m_Pages = new List<Image>();
    public int m_CurrentPage = 0;
    public float m_ContentFadingDuration;
    public AnimationCurve m_FadingMovement;
    
    [Header("Flip Action Settings")]
    public float m_FlipDuration;
    public float m_FastFlipDuration;
    public AnimationCurve m_FlipMovement;
    public Image m_PreviousPageLocation;
    public Image m_CurrentPageLocation;
    public Image m_NextPageLocation;
    public Image m_DummyPage;
    public string m_FlipPageAudioName;    
    public delegate void PageDel();
    public PageDel WhenPageFlipDone;
    [Header("Buttons")]
    public Button m_ButtonPrevious;
    public Button m_ButtonNext;
    [Header("Audio")]
    public string m_OpenNotebookAudioName;
    public string m_CloseNotebookAudioName;
    public string m_GetNotebookPageAudioName;

    Coroutine ImageFadingProgress { get; set; }
    Coroutine TMPFadingProgress { get; set; }
    Coroutine ShowPageProcess { get; set; }
    private void Awake()
    {
        Initialize();        
    }
    private void OnEnable()
    {
        UpdateButtonInteractBool();
        Belete.GameManager.Instance.m_AudioManager.PlayGlobalClip(m_OpenNotebookAudioName);
    }
    private void OnDisable()
    {
        Belete.GameManager.Instance.m_AudioManager.PlayGlobalClip(m_CloseNotebookAudioName);
    }
    private void Initialize() {
        
        foreach (var page in m_Pages)
        {
            if(page)
                page.gameObject.SetActive(false);
        }

        m_Pages[m_CurrentPage].gameObject.SetActive(true);        
    }
    #region Public Function
    public void NextPage() {
        if (ImageFadingProgress != null || TMPFadingProgress != null || m_CurrentPage >= m_Pages.Count - 1) return;

        int nextpage = m_CurrentPage + 1;

        if (!m_ButtonPrevious.interactable) m_ButtonPrevious.interactable = true;

        StartCoroutine(ChangePageSeq(true, m_CurrentPage, nextpage, m_FlipDuration, m_ContentFadingDuration));

        UpdatePageButtonState(m_Pages[nextpage]);

        Belete.GameManager.Instance.m_AudioManager.PlayGlobalClip(m_FlipPageAudioName);
    }
    public void PreviousPage() {
        if (ImageFadingProgress != null || TMPFadingProgress != null || m_CurrentPage - 1 < 0) return;
        
        int nextpage = m_CurrentPage - 1;

        if (!m_ButtonNext.interactable) m_ButtonNext.interactable = true; 

        StartCoroutine(ChangePageSeq(false, m_CurrentPage, nextpage, m_FlipDuration, m_ContentFadingDuration));
        
        UpdatePageButtonState(m_Pages[nextpage]);
        
        Belete.GameManager.Instance.m_AudioManager.PlayGlobalClip(m_FlipPageAudioName);
    }    
    public void AddPage(GameObject page) {
        var clone = Instantiate(page);

        var insertIndex = InsertPage(clone.GetComponent<Page>(), clone.GetComponent<Image>());

        var insertSiblingIndex = m_Pages[ Mathf.Clamp(insertIndex - 1, 0, int.MaxValue) ].transform.GetSiblingIndex();
        clone.transform.SetParent(m_PageContainer);
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localScale = Vector3.one;
        clone.transform.SetSiblingIndex(insertSiblingIndex);        
        Initialize();

        Belete.GameManager.Instance.m_AudioManager.PlayGlobalClip(m_GetNotebookPageAudioName);
    }
    public void ShowPage(int listIndex) {
        if (ShowPageProcess != null) StopCoroutine(ShowPageProcess);

        ShowPageProcess = StartCoroutine(FlipToPage(listIndex));
    }
    #endregion
    private int InsertPage(Page page, Image image) {

        if (m_Pages.Count < 2) {
            if (!m_Pages[0].TryGetComponent<Page>(out Page currentPage)) return -1;

            if (currentPage.m_ListIndex > page.m_ListIndex)
            { 
                m_Pages.Insert(0, image);  //print("insert to 0"); 
                ++m_CurrentPage;
                return 0;
            }
            else
            { 
                m_Pages.Add(image);  //print("Add"); 
                return 1;
            }
        }

        for (int i = 0; i < m_Pages.Count - 1; i++)
        {
            if (!m_Pages[i].TryGetComponent<Page>(out Page currentPage)) continue;
            if (!m_Pages[i + 1].TryGetComponent<Page>(out Page nextPage)) continue;

            if (page.m_ListIndex >= currentPage.m_ListIndex && page.m_ListIndex <= nextPage.m_ListIndex)
            {
                m_Pages.Insert(i + 1, image);
                //print(currentPage.m_ListIndex + " " + nextPage.m_ListIndex + ",Inset to " + (i + 1));
                
                if(i + 1 <= m_CurrentPage)
                    ++m_CurrentPage;

                return i + 1;
            }
        }
        //print("Add");

        // if all the page listIndex is smaller than page
        m_Pages.Add(image);

        return m_Pages.Count - 1;
    }
    private IEnumerator FlipToPage(int listIndex) {

        var targetIndex = SearchForPageIndex(listIndex);

        if (m_CurrentPage > targetIndex)
        {
            while (m_CurrentPage > targetIndex)
            {
                yield return StartCoroutine(ChangePageSeq(false,m_CurrentPage, m_CurrentPage - 1, m_FastFlipDuration, m_FastFlipDuration));
                EventHandler.WhenShowingAquirePage?.Invoke(true);
            }
        }
        else
        {
            while (m_CurrentPage < targetIndex)
            {
                yield return StartCoroutine(ChangePageSeq(true ,m_CurrentPage, m_CurrentPage + 1, m_FastFlipDuration, m_FastFlipDuration));                 
                EventHandler.WhenShowingAquirePage?.Invoke(true);                
            }
        }

        EventHandler.WhenShowingAquirePage?.Invoke(false);

        UpdateButtonInteractBool();

        ShowPageProcess = null;
    }  
    private IEnumerator ChangePageSeq(bool forward,int currentPage, int NextPage, float flipDuration, float fadeDuration)
    {
        var images = m_Pages[currentPage].GetComponentsInChildren<Image>();
        var textMeshPro = m_Pages[currentPage].GetComponentsInChildren<TextMeshProUGUI>();

        images = FilteredPageImage(images, m_Pages[currentPage].GetComponent<Image>());
        ImageFadingProgress = StartCoroutine(ImageFading(images, 1, 0, fadeDuration));
        TMPFadingProgress = StartCoroutine(TMPFading(textMeshPro, 1, 0, fadeDuration));

        var current = m_CurrentPageLocation;
        var target = (forward) ? m_PreviousPageLocation : m_NextPageLocation;
        StartCoroutine(SlidePage(m_Pages[currentPage], current, target, flipDuration));
        current = (forward) ? m_NextPageLocation : m_PreviousPageLocation;
        target = m_CurrentPageLocation;
        StartCoroutine(SlidePage(m_Pages[NextPage], current, target, flipDuration));

        var from = (forward) ? Vector3.one * .9f : m_PreviousPageLocation.rectTransform.localScale;
        var to = (forward) ? m_PreviousPageLocation.rectTransform.localScale : Vector3.one * .9f ;
        StartCoroutine(ScaleDummy(from, to, m_PreviousPageLocation.rectTransform.localScale, m_FlipDuration));

        int increaseOrDecrease = (forward) ? -1 : 1;
        m_Pages[NextPage].transform.SetSiblingIndex(m_Pages[currentPage].transform.GetSiblingIndex() + increaseOrDecrease);
        m_Pages[NextPage].gameObject.SetActive(true);

        //yield return new WaitUntil(() => (ImageFadingProgress == null));
        //yield return new WaitUntil(() => (TMPFadingProgress == null));

        images = m_Pages[NextPage].GetComponentsInChildren<Image>();
        textMeshPro = m_Pages[NextPage].GetComponentsInChildren<TextMeshProUGUI>();

        images = FilteredPageImage(images, m_Pages[NextPage].GetComponent<Image>());
        ImageFadingProgress = StartCoroutine(ImageFading(images, 0, 1, fadeDuration));
        TMPFadingProgress = StartCoroutine(TMPFading(textMeshPro, 0, 1, fadeDuration));

        yield return new WaitUntil(() => (ImageFadingProgress == null));
        yield return new WaitUntil(() => (TMPFadingProgress == null));

        m_Pages[currentPage].gameObject.SetActive(false);

        Belete.GameManager.Instance.m_AudioManager.PlayGlobalClip(m_FlipPageAudioName);

        WhenPageFlipDone?.Invoke();

        m_CurrentPage = NextPage;
        UpdateButtonInteractBool();
    }
    private Image[] FilteredPageImage(Image[] images, Image page) {
        List<Image> imageList = new List<Image>();

        for (int i = 0; i < images.Length; i++)
        {
            if(images[i] != page)
                imageList.Add(images[i]);
        }

        return imageList.ToArray();
    }
    private Page SearchForPage(int listIndex) {
        for (int i = 0; i < m_Pages.Count; i++)
        {
            if (!m_Pages[i].TryGetComponent<Page>(out Page page)) continue;

            if (page.m_ListIndex == listIndex) return page;
        }

        return null;
    }
    private int SearchForPageIndex(int listIndex)
    {
        for (int i = 0; i < m_Pages.Count; i++)
        {
            if (!m_Pages[i].TryGetComponent<Page>(out Page page)) continue;

            if (page.m_ListIndex == listIndex) return i;
        }

        return 0;
    }
    #region Flipping Page Transition
    private IEnumerator ScaleDummy(Vector3 from, Vector3 to, Vector3 finishScale, float duration) {
        var startTime = Time.time;
        var endTime = duration;

        while (Time.time - startTime < endTime)
        {
            m_DummyPage.rectTransform.localScale = Vector3.Lerp(from, to, m_FlipMovement.Evaluate((Time.time - startTime) / endTime));
            yield return null;
        }

        m_DummyPage.rectTransform.localScale = finishScale;
    }
    private IEnumerator SlidePage(Image page, Image currentLocation, Image targetLocation, float duration)
    {
        var startTime = Time.time;
        var endTime = duration;
        
        Color fromCol = currentLocation.color;
        Color toCol = targetLocation.color;

        Vector3 fromPos = currentLocation.rectTransform.position;
        Vector3 toPos = targetLocation.rectTransform.position;

        Quaternion fromRot = currentLocation.rectTransform.rotation;
        Quaternion toRot = targetLocation.rectTransform.rotation;

        float step;

        while (Time.time - startTime < endTime)
        {
            step = m_FlipMovement.Evaluate((Time.time - startTime) / endTime);

            page.color = Color.Lerp(fromCol, toCol, step);
            page.rectTransform.position = Vector3.Lerp(fromPos, toPos, step);
            page.rectTransform.rotation = Quaternion.Lerp(fromRot, toRot, step);
            yield return null;
        }

        page.color = toCol;
        page.rectTransform.position = toPos;
        page.rectTransform.rotation = toRot;
    }
    private IEnumerator ImageFading(Image[] images, float FromAlpha, float ToAlpha, float duration) {
        var startTime = Time.time;
        var endTime = duration;
                
        Color fromCol;        
        Color toCol;
        float step;

        while (Time.time - startTime < endTime)        
        {
            step = m_FadingMovement.Evaluate((Time.time - startTime) / endTime);

            for (int i = 0; i < images.Length; i++)
            {
                fromCol = images[i].color;
                toCol = images[i].color;
                
                fromCol.a = FromAlpha;
                toCol.a = ToAlpha;

                images[i].color = Color.Lerp(fromCol, toCol, step);         
            }

            yield return null;
        }

        for (int i = 0; i < images.Length; i++)
        {
            toCol = images[i].color;
            toCol.a = ToAlpha;
            images[i].color = toCol;
        }

        ImageFadingProgress = null;
    }
    private IEnumerator TMPFading(TextMeshProUGUI[] TMP, float FromAlpha, float ToAlpha, float duration) {
        var startTime = Time.time;
        var endTime = duration;

        Color fromCol;
        Color toCol;
        float step;

        while (Time.time - startTime < endTime)
        {
            step = m_FadingMovement.Evaluate((Time.time - startTime) / endTime);

            for (int i = 0; i < TMP.Length; i++)
            {
                fromCol = TMP[i].color;
                toCol = TMP[i].color;

                fromCol.a = FromAlpha;
                toCol.a = ToAlpha;

                TMP[i].color = Color.Lerp(fromCol, toCol, step);
            }

            yield return null;
        }

        for (int i = 0; i < TMP.Length; i++)
        {
            toCol = TMP[i].color;
            toCol.a = ToAlpha;
            TMP[i].color = toCol;
        }

        TMPFadingProgress = null;
    }
    #endregion
    #region Page Button Interaction Control
    private void UpdateButtonInteractBool()
    {        
        if (m_Pages.Count == 1) {
            m_ButtonNext.interactable = false;
            m_ButtonPrevious.interactable = false;

            return;
        }
        if (m_CurrentPage == m_Pages.Count - 1)
        {
            m_ButtonNext.interactable = false;
            m_ButtonPrevious.interactable = true;
        }
        if (m_CurrentPage == 0)
        {
            m_ButtonNext.interactable = true;
            m_ButtonPrevious.interactable = false;
        }
    }
    private void UpdatePageButtonState(Image page) {
        if (!page.TryGetComponent<PageInteractableControl>(out PageInteractableControl control)) return;

        control.SwitchButtons(true);
    }
    #endregion
}
