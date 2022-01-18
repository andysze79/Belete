using Belete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AndyAsset
{
    public class IsometricCharacterController : MonoBehaviour,Belete.IGameStateEvent
    {
        //public Rigidbody m_Rigidbody;
        public float m_Gravity = -9.8f;
        public CharacterController m_CharactorController;
        public float m_Speed;

        public string m_WalkingAudioName;

        private AudioSource audioSource;

        bool active = true;

        private void Awake()
        {
            m_CharactorController = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();
        }
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
            if (!active) {
                audioSource.Stop();
                return; 
            }

            var moveDir = new Vector3(horizontalValue * m_Speed * Time.deltaTime, m_Gravity * Time.deltaTime, verticalValue * m_Speed * Time.deltaTime);
            //m_Rigidbody.velocity = new Vector3(horizontalValue * m_Speed * Time.deltaTime, m_Rigidbody.velocity.y, verticalValue * m_Speed * Time.deltaTime);
            m_CharactorController.Move(moveDir);

            if (horizontalValue != 0 || verticalValue != 0)
            {
                Belete.GameManager.Instance.m_AudioManager.PlayContinousClip(audioSource, m_WalkingAudioName);
            }
            else {
                audioSource.Stop();
            }
        }

        public void OnNotifyGameStateChanged(GameManager.GameState state)
        {
            active = (state == GameManager.GameState.Adventure);
        }
    }
}
