using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class CheckInteractable : MonoBehaviour
{
    public TriggerBase m_Trigger;
    public LayerMask m_Layer;
    public LayerMask m_CheckBlockerLayer;
    public bool m_DebugMode = false;
    private Collider triggerCollider;
    private void Awake()
    {
         triggerCollider = m_Trigger.GetComponent<Collider>();
    }
    private void OnEnable()
    {
        m_Trigger.TriggerEnter += ActivateInteractable;
        m_Trigger.TriggerExit += DeactivateInteractable;
        m_Trigger.m_TargetLayer = m_Layer;
        EventHandler.WhenSwitchInteractableSimple += UpdateCollider;
    }

    private void OnDisable()
    {
        m_Trigger.TriggerEnter -= ActivateInteractable;
        m_Trigger.TriggerExit -= DeactivateInteractable;        
        EventHandler.WhenSwitchInteractableSimple -= UpdateCollider;
    }
    private void ActivateInteractable(Collider other) {

        if (hasBlocker(other)) return;

        var interactable = other.GetComponent<IInteractable>();

        interactable.InRange();
    }
    private void DeactivateInteractable(Collider other)
    {
        var interactable = other.GetComponent<IInteractable>();

        interactable.OutOfRange();
    }
    private void UpdateCollider()
    {
        triggerCollider.enabled = false;
        triggerCollider.enabled = true;
    }
    private bool hasBlocker(Collider target) {
        var dir = (target.transform.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hitInfo;
        float dist = Vector3.Distance(transform.position, target.transform.position);

        if (m_DebugMode)
        {
            print("Get " + target.name);
            Debug.DrawRay(ray.origin, ray.direction, Color.blue);
            Debug.DrawLine(transform.position, target.transform.position, Color.cyan);
        }

        if (Physics.Raycast(ray, out hitInfo, dist * .95f, m_CheckBlockerLayer))
        {
            if(m_DebugMode) print("Block by " + hitInfo.collider.name);
            return true; 
        }
        else
            return false;
    }
}
