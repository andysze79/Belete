using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : TriggerBase
{
    public MeshRenderer[] m_EnableTheseWhenEnter;
    public MeshRenderer[] m_DisableTheseWhenEnter;
    public GameObject m_Camera;
    public MeshRenderer m_Floor;
    public GameObject[] m_Lights;
    private void OnEnable()
    {
        TriggerEnter += PlayerEnter;
        TriggerExit += PlayerExit;            
    }
    private void OnDisable()
    {
        TriggerEnter -= PlayerEnter;
        TriggerExit -= PlayerExit;
    }
    private void PlayerEnter(Collider other) 
    {
        TurnOnOffMesh(true, m_EnableTheseWhenEnter);
        TurnOnOffMesh(false, m_DisableTheseWhenEnter);
        if (m_Floor) m_Floor.enabled = true;
        TurnOnOffObjs(true, m_Lights);
        if (m_Camera) m_Camera.SetActive(true);
    }
    private void PlayerExit(Collider other)
    {
        TurnOnOffMesh(false, m_EnableTheseWhenEnter);
        TurnOnOffMesh(true, m_DisableTheseWhenEnter);
        if(m_Floor) m_Floor.enabled = false;
        TurnOnOffObjs(false, m_Lights);
        if (m_Camera) m_Camera.SetActive(false);
    }
    private void TurnOnOffMesh(bool value, MeshRenderer[] mesh) {
        if (mesh.Length == 0) return;

        for (int i = 0; i < mesh.Length; i++)
        {
            mesh[i].enabled = value;
        }
    }
    private void TurnOnOffObjs(bool value, GameObject[] objs)
    {
        if (objs.Length == 0) return;

        for (int i = 0; i < objs.Length; i++)
        {
            objs[i].SetActive(value);
        }
    }

}
