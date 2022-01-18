using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LightControl : MonoBehaviour
{
    public List<Light> m_Lights;
    public float m_Duration;
    public AnimationCurve m_Movement;
    private List<float> originalIntensity = new List<float>();

    Coroutine process { get; set; }

    private void Awake()
    {
        for (int i = 0; i < m_Lights.Count; i++)
        {            
            originalIntensity.Add(m_Lights[i].intensity);
        }
    }
    public void ChangeIntensityToOriginal()
    {        
        if (process != null) StopCoroutine(process);

        process = StartCoroutine(Transition(originalIntensity, m_Duration, m_Movement));
    }
    public void ChangeIntensityFromZeroToDefault()
    {
        for (int i = 0; i < m_Lights.Count; i++)
        {
            var to = m_Lights[i].intensity;
            m_Lights[i].intensity = 0;
            m_Lights[i].DOIntensity(to, m_Duration).SetEase(m_Movement);
        }
    }
    public void ChangeIntensity(float value) {
        List<float> values = new List<float>();

        for (int i = 0; i < m_Lights.Count; i++)
        {
            values.Add(value);
        }

        if (process != null) StopCoroutine(process); 

        process = StartCoroutine(Transition(values, m_Duration, m_Movement));
    }
    private IEnumerator Transition(List<float> value, float duration, AnimationCurve movement) {
        var startTime = Time.time;
        var endTime = duration;
        List<float> from = new List<float>();
        float step;

        for (int i = 0; i < m_Lights.Count; i++)
        {
            from.Add(m_Lights[i].intensity);
        }

        while (Time.time - startTime < endTime)
        {            
            step = m_Movement.Evaluate((Time.time - startTime) / endTime);

            for (int i = 0; i < m_Lights.Count; i++)
            {
                m_Lights[i].intensity = Mathf.Lerp(from[i], value[i], step);
            }

            yield return null;
        }

        for (int i = 0; i < m_Lights.Count; i++)
        {
            m_Lights[i].intensity = value[i];
        }
    }
}
