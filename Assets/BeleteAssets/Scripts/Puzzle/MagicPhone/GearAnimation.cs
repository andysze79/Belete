using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GearAnimation : MonoBehaviour
{
    public float m_Speed;
    private float increment;
    public bool Active;
    public void ActivateGears(bool value) {
        Active = value;
    }
    private void Update() {
        if (!Active) return;

        increment += Time.deltaTime * m_Speed;

        transform.localEulerAngles = Vector3.Lerp(
             new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0f),
             new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 360f),
             (increment >= 0)? (increment * 10 % 10f) / 10 : 1 - Mathf.Abs((increment * 10 % 10f) / 10)
             );
    }    
}
