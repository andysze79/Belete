using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource m_GlobalAudioSource;
    Coroutine Process;
    private void Awake()
    {
        m_GlobalAudioSource.GetComponent<AudioSource>();
    }
    public void PlayGlobalClip(string clip)
    {
        m_GlobalAudioSource.PlayOneShot(Belete.GameManager.Instance.m_AudioLibrary.GetAudio(clip).Clip);
    }
    public void PlayClip(AudioSource source, string clip)
    {        
        source.PlayOneShot(Belete.GameManager.Instance.m_AudioLibrary.GetAudio(clip).Clip);
    }
    public void PlayClip(AudioSource source, AudioClip clip) {
        source.PlayOneShot(clip);
    }
    public void PlayContinousClip(AudioSource source, string clip) 
    {
        if(!source.isPlaying)
            source.PlayOneShot(Belete.GameManager.Instance.m_AudioLibrary.GetAudio(clip).Clip);
    }
}
