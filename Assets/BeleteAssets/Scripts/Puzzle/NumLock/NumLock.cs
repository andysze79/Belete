using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class NumLock : PuzzleSubject
{    
    public float m_RotateStep = 30;
    public float m_RotateDuration = .5f;
    public AnimationCurve m_RotateMovement;
    public List<Transform> m_NumberObjs = new List<Transform>();
    public List<float> m_NumberToRotation = new List<float>();
    public List<int> m_CorrectNumber = new List<int>();
    public string m_RotateAudioName; 
    public UltEvent WhenFirstTimeUse;
    public UltEvent WhenRotate;
    public UltEvent WhenCorrect;
    [Header("Debug")]
    public bool m_FixThePuzzleRightAway;
    [SerializeField]private List<int> currentNumber = new List<int>();
    bool firstTimeUse = true;
    bool unlocked { get; set; }
    Coroutine rotateProcess { get; set; }
    private void Awake()
    {
        for (int i = 0; i < m_NumberObjs.Count; i++)
        {
            m_NumberObjs[i].GetComponent<NumLockParts>().numLock = this;
        }

        for (int i = 0; i < m_NumberObjs.Count; i++)
        {
            currentNumber.Add(RotationToNumber(m_NumberObjs[i].eulerAngles.y));
        }
    }
    private void OnEnable()
    {
        WhenRotate += PlayRotateSound;

        if (firstTimeUse)
        {
            firstTimeUse = false;
            WhenFirstTimeUse?.Invoke();
        }
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
 
        WhenRotate -= PlayRotateSound;
    }
    private void Update()
    {
        if (m_FixThePuzzleRightAway)
        {
            m_FixThePuzzleRightAway = false;
            WhenCorrect?.Invoke();
            NotifyRequestorEvent();
        }
    }
    public void OnInteract(Transform target) {
        if (unlocked) return;

        for (int i = 0; i < m_NumberObjs.Count; i++)
        {
            if (m_NumberObjs[i] == target)
            {
                if (rotateProcess != null) return;                   

                rotateProcess = StartCoroutine(RotateNumObj(target)); 
            }
        }
    }
    private void PlayRotateSound() {
        Belete.GameManager.Instance.m_AudioManager.PlayGlobalClip(m_RotateAudioName);
    }
    private IEnumerator RotateNumObj(Transform target) {
        var startTime = Time.time;
        var endTime = m_RotateDuration;
        var from = target.localRotation;
        var to = Quaternion.Euler(0, target.localEulerAngles.y + m_RotateStep,0);
        
        WhenRotate?.Invoke();                

        while (Time.time - startTime < endTime)
        {
            target.localRotation = Quaternion.Lerp(from, to, m_RotateMovement.Evaluate((Time.time - startTime) / endTime));

            yield return null;
        }
        target.localRotation = to;

        UpdateCurrentNumber();

        if (CheckCorrectNumber())
        {
            unlocked = true;
            WhenCorrect?.Invoke();
            NotifyRequestorEvent();
        }

        rotateProcess = null;
    }
    private void UpdateCurrentNumber()
    {
        for (int i = 0; i < m_NumberObjs.Count; i++)
        {
            currentNumber[i] = RotationToNumber(m_NumberObjs[i].localEulerAngles.y);
        }
    }
    private int RotationToNumber(float eularAngleY) {
        int result = -1;
        for (int i = 0; i < m_NumberToRotation.Count; i++)
        {
            //print(Mathf.Abs(Mathf.Abs(eularAngleY % 360f) - m_NumberToRotation[i]));
            if (Mathf.Abs(Mathf.Abs(eularAngleY % 360f) - m_NumberToRotation[i]) < 1)
                result = i;
        }

        return result;
    }
    private bool CheckCorrectNumber() { 
        bool result = true;

        for (int i = 0; i < currentNumber.Count; i++)
        {
            if (currentNumber[i] != m_CorrectNumber[i])
                result = false;
        }

        return result;
    }
}
