using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public TMP_Text label;

    public void Start()
    {
        label.text = $"Вы вошли под ником {Account.Nickname}";
        
        AudioStatic.AddSoundsToButtons(AudioStatic.Button, gameObject);
    }

    public void Play() => SceneManager.LoadScene("GameScene");

    public void Exit()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void EnterCollection() => SceneManager.LoadScene("CollectionScene");

    public void EnterSettings() => SceneManager.LoadScene("SettingsScene");

    public void Logout()
    {
        Account.Reset();
        SceneManager.LoadScene("LoginScene");
    }

    public void Shop() => SceneManager.LoadScene("ShopScene");
}