using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
[RequireComponent(typeof(SphereCollider))]
public class SimpleRotater : MonoBehaviour
{
    public float m_RotateSpeed;
    public float m_ResetDuration;
    public Ease m_ResetMovement;
    private Vector3 DownPos;
    private Vector3 CurrentRot;
    private Vector3 OriginalRot;
    private void Awake()
    {
        OriginalRot= transform.localEulerAngles;        
    }
    private void OnEnable()
    {
        ResetRotation(0);        
    }    
    private void OnMouseDown()
    {
        DownPos = Input.mousePosition;
        CurrentRot = transform.localEulerAngles;
        //print("On mouse down");
    }
    private void OnMouseDrag()
    {
        //print("On mouse drag");
        Vector3 dir = (Input.mousePosition - DownPos);

        var Rot = transform;
        
        Rot.localEulerAngles = CurrentRot + m_RotateSpeed * new Vector3(dir.y, -dir.x , 0);        
    }
    private void OnMouseUp()
    {
        
    }
    private void ResetRotation(float duration)
    {
        transform.DOLocalRotate(OriginalRot, duration).SetEase(m_ResetMovement);
    }
    public void ResetRotationButton()
    {
        ResetRotation(m_ResetDuration);
    }
}
