using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicPhone : PuzzleSubject, IHasCoolDown
{
    public TunerKey[] m_TunerKeys;
    public int m_InsertedOrbsAmount;
    public int m_RequireOrbsAmount = 3;
    public bool m_FinishedTuning;
    public float m_KeyToNeighborsInterval = .3f;
    [Header("Cooldown Settings")]
    public CoolDownSystem m_CooldownSystem;
    public int m_Id = 1;
    public float m_CooldownDuration = .3f;
    [Header("Debug")]
    public bool m_FixThePuzzleRightAway;
    public int Id => m_Id;
    public float CooldownDuration => m_CooldownDuration;
    private bool AlreadyIntroduced;
    public UltEvents.UltEvent WhenFirstIntroduce;
    public UltEvents.UltEvent WhenNotYetInsertedAllOrbs;
    public UltEvents.UltEvent WhenInsertedAllOrbs;
    public UltEvents.UltEvent WhenFinishedTuning;

    private void OnEnable()
    {
        m_TunerKeys = GetComponentsInChildren<TunerKey>();
        
        for (int i = 0; i < m_TunerKeys.Length; i++)
        {
            m_TunerKeys[i].WhenTunerKeyPressed += OnTunerKeyPressed;
        }

        if (!AlreadyIntroduced) {
            AlreadyIntroduced = true;
            WhenFirstIntroduce?.Invoke();
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        for (int i = 0; i < m_TunerKeys.Length; i++)
        {
            m_TunerKeys[i].WhenTunerKeyPressed -= OnTunerKeyPressed;
        }
    }
    private void Update()
    {
        if (m_FixThePuzzleRightAway) {
            m_FixThePuzzleRightAway = false;
            WhenFinishedTuning?.Invoke();
            NotifyRequestorEvent();
        }
    }
    private void OnTunerKeyPressed(TunerKey key, TunerKey[] neighbors) {

        if (!CheckAllOrbsInserted()) { WhenNotYetInsertedAllOrbs?.Invoke(); return; }
        if (m_FinishedTuning) return;
        if (m_CooldownSystem.IsOnCooldown(m_Id)) return;

        StartCoroutine(HandlingTunerKeys(key, neighbors));
        m_CooldownSystem.PutOnCooldown(this);
    }
    private IEnumerator HandlingTunerKeys(TunerKey key, TunerKey[] neighbors) {
        key.m_Active = !key.m_Active;
        key.m_Light.SetActive(!key.m_Light.activeSelf);

        yield return new WaitForSeconds(m_KeyToNeighborsInterval);

        for (int i = 0; i < neighbors.Length; i++)
        {
            neighbors[i].m_Active = !neighbors[i].m_Active;
            neighbors[i].m_Light.SetActive(!neighbors[i].m_Light.activeSelf);
            neighbors[i].OnInteract();
        }

        if (CheckAllActive())
        { 
            m_FinishedTuning = true;
            WhenFinishedTuning?.Invoke();
            NotifyRequestorEvent();
        }
    }
    private bool CheckAllActive() {
        
        for (int i = 0; i < m_TunerKeys.Length; i++)
        {
            if (!m_TunerKeys[i].m_Active)
                return false;
        }

        return true;
    }
    public void InsertedOrb() {
        ++m_InsertedOrbsAmount;

        if (CheckAllOrbsInserted())
            WhenInsertedAllOrbs?.Invoke();
    }
    private bool CheckAllOrbsInserted()
    {
        return m_InsertedOrbsAmount >= m_RequireOrbsAmount;
    }
}
