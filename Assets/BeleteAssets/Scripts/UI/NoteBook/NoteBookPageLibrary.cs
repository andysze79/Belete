using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Belete/ Note Book Page Library")]
public class NoteBookPageLibrary : ScriptableObject
{
    public List<Page> m_AllNoteBookPages = new List<Page>();

    public Page GetPage(string name)
    {
        Page target = m_AllNoteBookPages[0];
        for (int i = 0; i < m_AllNoteBookPages.Count; i++)
        {
            if (m_AllNoteBookPages[i].m_PageName == name)
                target = m_AllNoteBookPages[i];
        }

        return target;
    }
}
