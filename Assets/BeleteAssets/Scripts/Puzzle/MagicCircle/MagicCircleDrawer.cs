using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MagicCircleDrawer : MonoBehaviour
{
    public enum CurrentDrawingParts { 
        outer, middle, inner, enchant
    }
    public CurrentDrawingParts m_CurrentDrawingParts = CurrentDrawingParts.outer;
    public Color m_DefaultColor = Color.white;
    [BoxGroup("Assets Settings")] public Sprite[] m_OuterOptions;
    [BoxGroup("Assets Settings")] public Sprite[] m_MiddleOptions;
    [BoxGroup("Assets Settings")] public Sprite[] m_InnerOptions;
    [BoxGroup("Assets Settings")] public Sprite[] m_EnchantOptions;
    [BoxGroup("Assets Settings")] public Color[] m_EnchantColors;
    [BoxGroup("UI Settings")] public Image[] m_OptionsImage;
    [BoxGroup("UI Settings")] public Image m_OuterImage;
    [BoxGroup("UI Settings")] public Image m_MiddleImage;
    [BoxGroup("UI Settings")] public Image m_InnerImage;

    public float m_EndDrawingEffectDuration = 1f;

    public List<int> result = new List<int>();

    public UltEvents.UltEvent WhenFirstTimeUse;
    public UltEvents.UltEvent WhenFinishedDrawing;

    public delegate void DrawingDel();
    public DrawingDel FinishedDrawingDel;
    public RequestMagicCircle requester { get; set; }
    public bool once { get; set; }
    public static MagicCircleDrawer m_Instance;
    public static MagicCircleDrawer instance {
        get {
            if (m_Instance == null)
                m_Instance = GameObject.FindObjectOfType<MagicCircleDrawer>();

            return m_Instance;
        }
    }
        
    private void OnEnable()
    {
        if (!once)
        {
            once = true;
            WhenFirstTimeUse?.Invoke(); 
        }

        Init();
    }
    private void Init() {
        m_CurrentDrawingParts = 0;
        m_InnerImage.sprite = null;
        m_OuterImage.sprite = null;
        m_MiddleImage.sprite = null;
        m_InnerImage.color = m_DefaultColor;
        m_OuterImage.color = m_DefaultColor;
        m_MiddleImage.color = m_DefaultColor;
        DG.Tweening.DOTweenModuleUI.DOFade(m_InnerImage, 0, 0);
        DG.Tweening.DOTweenModuleUI.DOFade(m_OuterImage, 0, 0);
        DG.Tweening.DOTweenModuleUI.DOFade(m_MiddleImage, 0, 0);
        
        for (int i = 0; i < m_OptionsImage.Length; i++)
        {
            m_OptionsImage[i].enabled = false;
            DG.Tweening.DOTweenModuleUI.DOFade(m_OptionsImage[i], 0, 0f);
            m_OptionsImage[i].GetComponent<Button>().interactable = true;
        }

        result.Clear();

        StartCoroutine(UpdateOptions());
    }
    /// <summary>
    /// Button event
    /// </summary>
    /// <param name="index"></param>
    public void ChooseImage(int index) {

        if (m_CurrentDrawingParts > CurrentDrawingParts.enchant) return;

        result.Add(index);

        switch (m_CurrentDrawingParts)
        {
            case CurrentDrawingParts.outer:
                m_OuterImage.sprite = m_OuterOptions[index]; 
                DG.Tweening.DOTweenModuleUI.DOFade(m_OuterImage, 1, .5f);
                break;
            case CurrentDrawingParts.middle:
                m_MiddleImage.sprite = m_MiddleOptions[index];                
                DG.Tweening.DOTweenModuleUI.DOFade(m_MiddleImage, 1, .5f);
                break;
            case CurrentDrawingParts.inner:
                m_InnerImage.sprite = m_InnerOptions[index];                
                DG.Tweening.DOTweenModuleUI.DOFade(m_InnerImage, 1, .5f);
                break;
            case CurrentDrawingParts.enchant:
                DG.Tweening.DOTweenModuleUI.DOColor(m_InnerImage, m_EnchantColors[index], 1f);
                DG.Tweening.DOTweenModuleUI.DOColor(m_MiddleImage, m_EnchantColors[index], 1f);
                DG.Tweening.DOTweenModuleUI.DOColor(m_OuterImage, m_EnchantColors[index], 1f);
                break;
            default:
                break;
        }

        if (m_CurrentDrawingParts < CurrentDrawingParts.enchant)
        { 
            ++m_CurrentDrawingParts;
            StartCoroutine(UpdateOptions());            
        }
        else
            StartCoroutine(EndDrawing());
    }
    private IEnumerator UpdateOptions()
    {
        for (int i = 0; i < m_OptionsImage.Length; i++)
        {
            DG.Tweening.DOTweenModuleUI.DOFade(m_OptionsImage[i], 0, .3f);
        }
        yield return new WaitForSeconds(.3f);

        switch (m_CurrentDrawingParts)
        {
            case CurrentDrawingParts.outer:
                for (int i = 0; i < m_OptionsImage.Length; i++)
                {
                    m_OptionsImage[i].sprite = m_OuterOptions[i];
                } 
                break;
            case CurrentDrawingParts.middle:
                for (int i = 0; i < m_OptionsImage.Length; i++)
                {
                    m_OptionsImage[i].sprite = m_MiddleOptions[i];
                }
                break;
            case CurrentDrawingParts.inner:
                for (int i = 0; i < m_OptionsImage.Length; i++)
                {
                    m_OptionsImage[i].sprite = m_InnerOptions[i];
                }
                break;
            case CurrentDrawingParts.enchant:
                for (int i = 0; i < m_OptionsImage.Length; i++)
                {
                    m_OptionsImage[i].sprite = m_EnchantOptions[i];
                }
                break;
            default:
                break;
        }
        for (int i = 0; i < m_OptionsImage.Length; i++)
        {
            m_OptionsImage[i].enabled = true;
            DG.Tweening.DOTweenModuleUI.DOFade(m_OptionsImage[i], 1, .3f);
        }
    }
    private IEnumerator EndDrawing() {
        for (int i = 0; i < m_OptionsImage.Length; i++)
        {
            DG.Tweening.DOTweenModuleUI.DOFade(m_OptionsImage[i], 0, .3f);
            m_OptionsImage[i].GetComponent<Button>().interactable = false;
        }

        yield return new WaitForSeconds(m_EndDrawingEffectDuration);

        FinishedDrawingDel?.Invoke();

        requester?.OnNotify(result);

        requester = null;

        WhenFinishedDrawing?.Invoke();
    }        
}
