using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(InteractableSimple), typeof(ItemFunctions))]
public class InteractableAudioSettings : MonoBehaviour
{
    public string m_OnInteractAudioName;
    public string m_ReceiveItemAudioName;    

    private AudioSource audioSource;
    private InteractableSimple interactable;
    private ItemFunctions itemFunction;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        interactable = GetComponent<InteractableSimple>();
        itemFunction = GetComponent<ItemFunctions>();                
    }
    private void OnEnable()
    {
        if(m_OnInteractAudioName != "")
            interactable.WhenInteract += PlayInteractAudio;
        if (m_ReceiveItemAudioName!= "")
            itemFunction.WhenGainItem += PlayGainItemAudio;
    }
    private void OnDisable()
    {
        if (m_OnInteractAudioName != "")
            interactable.WhenInteract -= PlayInteractAudio;
        if (m_ReceiveItemAudioName != "") 
            itemFunction.WhenGainItem -= PlayGainItemAudio;
    }
    private void PlayInteractAudio() 
    {
        Belete.GameManager.Instance.m_AudioManager.PlayClip(audioSource, m_OnInteractAudioName);
    }
    private void PlayGainItemAudio()
    {
        Belete.GameManager.Instance.m_AudioManager.PlayClip(audioSource, m_ReceiveItemAudioName);
    }
}
