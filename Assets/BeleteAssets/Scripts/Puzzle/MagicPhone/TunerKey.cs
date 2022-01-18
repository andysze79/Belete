using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TunerKey : MonoBehaviour, IInteractable
{
    public TunerKey[] m_Neighbors;
    public bool m_Active;
    public delegate void TunerKeyDel(TunerKey key, TunerKey[] neighbors);
    public TunerKeyDel WhenTunerKeyPressed;
    public GameObject m_Light;
    public Vector3 m_RotateFrom;
    public Vector3 m_RotateTo;
    public float m_RotateDuration = .3f;
    public HighlightPlus.HighlightEffect m_HighlightEffect;
    public void InRange()
    {
        
    }

    public void OnHoverEnter()
    {
        m_HighlightEffect.highlighted = true;
    }

    public void OnHoverExit()
    {
        m_HighlightEffect.highlighted = false;
    }

    public void OnInteract()
    {
        print("Click");
        WhenTunerKeyPressed?.Invoke(this, m_Neighbors);

        if(m_Active)
            transform.DOLocalRotate(m_RotateTo, m_RotateDuration, RotateMode.Fast);
        else
            transform.DOLocalRotate(m_RotateFrom, m_RotateDuration, RotateMode.Fast);
    }

    public void OutOfRange()
    {
        
    }
}
