using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Belete/ Sound Library")]
public class AudioLibrary : ScriptableObject
{
    public List<AudioData> m_SoundList = new List<AudioData>();

    public AudioData GetAudio(string name)
    {
        AudioData target = m_SoundList[0];
        for (int i = 0; i < m_SoundList.Count; i++)
        {
            if (m_SoundList[i].Name == name)
                target = m_SoundList[i];
        }

        return target;
    }
}
