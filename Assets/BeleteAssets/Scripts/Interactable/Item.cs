using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[System.Serializable]
public class Item
{
    public string Name;
    public string UIName;
    [FoldoutGroup("Details")] public string Description;
    [FoldoutGroup("Details")] public Sprite Image;
    [FoldoutGroup("Details")] public int Amount;
}
