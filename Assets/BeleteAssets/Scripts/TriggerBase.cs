using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBase : MonoBehaviour
{

    public string m_Tag = "";
    public LayerMask m_TargetLayer;

    public delegate void TriggerDelegate(Collider target);
    public TriggerDelegate TriggerEnter;
    public TriggerDelegate TriggerExit;

    public float m_DelayTime = 1f;
    Coroutine Process { get; set; }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == m_Tag || 1 << other.gameObject.layer == m_TargetLayer) {
            TriggerEnter?.Invoke(other);
        }
    }
    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.tag == m_Tag || 1 << other.gameObject.layer == m_TargetLayer)
        {

        }
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.tag == m_Tag || 1 << other.gameObject.layer == m_TargetLayer)
        {
            TriggerExit?.Invoke(other); 
        }
    }
}
