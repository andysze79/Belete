using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string m_NextScene;
    public void ChangeToNextScene() { 
        SceneManager.LoadScene(m_NextScene);        
    }
    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
