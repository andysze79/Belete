using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingCamera : MonoBehaviour
{
    public bool m_Xfacing, m_Yfacing, m_Zfacing;
    public Transform MainCamera { get; set; }
    Vector3 pos;
    private void Awake()
    {
        MainCamera = Camera.main.transform;
    }
    private void Update()
    {
        pos.x = m_Xfacing ? transform.position.x : MainCamera.transform.position.x;
        pos.y = m_Yfacing ? transform.position.y : MainCamera.transform.position.y;
        pos.z = m_Zfacing ? transform.position.z : MainCamera.transform.position.z;

        transform.forward = -(pos - transform.position).normalized;
    }
}
