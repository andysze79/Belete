using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class AudioData
{
    [HorizontalGroup("Group 1", LabelWidth = 60)]
    public string Name;
    [HorizontalGroup("Group 1")]
    public AudioClip Clip;
}
