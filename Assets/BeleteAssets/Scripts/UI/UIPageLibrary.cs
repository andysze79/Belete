using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class UIPageData
{
    public string Id;
    public GameObject UIPage;
    public Belete.GameManager.GameState ChangeToState = Belete.GameManager.GameState.ViewingUI;
    public GameObject ClonePage { get; set; }
}
[CreateAssetMenu(menuName = "Belete/UIPageLibrary")]
public class UIPageLibrary : ScriptableObject
{
    public List<UIPageData> m_UIPages = new List<UIPageData>();

    public UIPageData GetPageData(string id) {
        for (int i = 0; i < m_UIPages.Count; i++)
        {
            if (m_UIPages[i].Id == id)
                return m_UIPages[i];
        }

        return null;
    }
}
