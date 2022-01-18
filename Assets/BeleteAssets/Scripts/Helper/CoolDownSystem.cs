using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownSystem : MonoBehaviour
{
    private readonly List<CooldownData> cooldowns = new List<CooldownData>();
    private void Update()
    {
        ProcessCooldowns();
    }

    private void ProcessCooldowns()
    {
        float deltaTime = Time.deltaTime;

        for (int i = cooldowns.Count - 1; i >= 0; i--)
        {
            if (cooldowns[i].DecrementCooldown(deltaTime)) {
                cooldowns.RemoveAt(i);
            }
        }
    }
    public void PutOnCooldown(IHasCoolDown cooldown) {
        cooldowns.Add(new CooldownData(cooldown));
    }
    public bool IsOnCooldown(int id) {
        foreach (CooldownData cooldown in cooldowns)
        {
            if (cooldown.Id == id) {
                return true;
            }
        }

        return false;
    }
    public float GetRemainingDuration(int id) {
        foreach (CooldownData cooldown in cooldowns)
        {
            if (cooldown.Id == id) {
                return cooldown.RemainingTime;
            }
        }
        return 0f;
    }
}
public class CooldownData {
    public CooldownData(IHasCoolDown coolDown) {
        Id = coolDown.Id;
        RemainingTime = coolDown.CooldownDuration;
    }
    public int Id { get; }
    public float RemainingTime { get; private set; }

    public bool DecrementCooldown(float deltaTime) {
        RemainingTime = Mathf.Max(RemainingTime - deltaTime, 0f);
        return RemainingTime == 0f;
    }
}
public interface IHasCoolDown {
    int Id { get; }
    float CooldownDuration { get;}
}
