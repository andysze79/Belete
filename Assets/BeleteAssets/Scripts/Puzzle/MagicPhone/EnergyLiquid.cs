using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnergyLiquid : MonoBehaviour
{
    public float m_From;
    public float m_To;
    public float m_Duration;
    public Ease m_Movement;
    private bool Active;
        
    public void FillUp(bool activeValue) {
        Active = activeValue;
        float target = activeValue ? m_To : m_From;
        transform.DOScaleY( target, m_Duration).SetEase(m_Movement);
    }
}
