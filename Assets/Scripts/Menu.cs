using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Exit()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
    
    public void EnterCollection()
    {
        SceneManager.LoadScene("CollectionScene");
    }

    public void EnterDevMode()
    {
        SceneManager.LoadScene("DeveloperScene");
    }

    public void EnterSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }
}
