using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestMagicCircle : MonoBehaviour
{
    public int[] m_RequestThisMagicCircle;
    public bool answer;
    public UltEvents.UltEvent WhenCorrect;
    public UltEvents.UltEvent WhenIncorrect;
    public void SendMagicCircleRequest() {
        MagicCircleDrawer.instance.requester = this;
    }    
    public void OnNotify(List<int> magicCircleResult) {
        var result = true;

        for (int i = 0; i < m_RequestThisMagicCircle.Length; i++)
        {
            if (m_RequestThisMagicCircle[i] != magicCircleResult[i])
                result = false;
        }

        answer = result;

        CheckAnswer();
    }
    private void CheckAnswer() {
        if (answer)
            WhenCorrect?.Invoke();
        else
            WhenIncorrect?.Invoke();
    }
}
