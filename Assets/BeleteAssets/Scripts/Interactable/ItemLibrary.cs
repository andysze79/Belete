using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Belete/ Item Library")]
public class ItemLibrary : ScriptableObject
{
    public List<Item> m_ItemList = new List<Item>();

    public Item GetItem(string name) {
        Item target = m_ItemList[0];
        for (int i = 0; i < m_ItemList.Count; i++)
        {
            if (m_ItemList[i].Name == name)
                target = m_ItemList[i];
        }

        return target;
    }
}
