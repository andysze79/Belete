using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientAudioSettings : MonoBehaviour
{
    public string m_AudioName;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Belete.GameManager.Instance.m_AudioManager.PlayClip(audioSource, m_AudioName);
    }
}
