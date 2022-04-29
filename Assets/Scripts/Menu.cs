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
        
        AudioStatic.AddMainTheme(AudioStatic.MainTheme, gameObject);
        AudioStatic.AddSoundsToButtons(AudioStatic.Click, gameObject);
    }

    public void Play() => SceneManager.LoadScene("GameScene");

    public void Exit()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void EnterCollection()
    {
        AudioStatic.RememberThemeState(gameObject);
        SceneManager.LoadScene("CollectionScene");
    }

    public void EnterSettings()
    {
        AudioStatic.RememberThemeState(gameObject);
        SceneManager.LoadScene("SettingsScene");
    }

    public void Logout()
    {
        Account.Reset();
        AudioStatic.RememberThemeState(gameObject);
        SceneManager.LoadScene("LoginScene");
    }

    public void Shop()
    {
        AudioStatic.RememberThemeState(gameObject);
        SceneManager.LoadScene("ShopScene");
    }
}