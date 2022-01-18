using Belete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorAnimationController : MonoBehaviour, Belete.IGameStateEvent
{
    public Animator m_Animtor;
    bool active = true;
    private void OnEnable()
    {
        Belete.GameManager.Instance.AddNotifyMember(this);
        InputManager.Instance.GetAxisAValue += Move;
    }

    private void OnDisable()
    {
        Belete.GameManager.Instance.RemoveNotifyMember(this);
        InputManager.Instance.GetAxisAValue -= Move;
    }
    private void Move(float horizontalValue, float verticalValue)
    {
        if (!active)
        {
            m_Animtor.speed = 0;
            return;
        }

        m_Animtor.SetFloat("Horizontal", horizontalValue);
        m_Animtor.SetFloat("Vertical", verticalValue);

        if (horizontalValue == 0 && verticalValue ==0)
        {
            m_Animtor.speed = 0;
        }
        else { 
            m_Animtor.speed = 1;        
        }
    }

    public void OnNotifyGameStateChanged(GameManager.GameState state)
    {
        active = (state == GameManager.GameState.Adventure);
    }
}
