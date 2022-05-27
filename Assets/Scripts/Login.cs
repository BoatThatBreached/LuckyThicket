using System;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    private const string CredentialsPath = "credentials";
    public TMP_Text duckPassword;
    public Popup popup;
    public TMP_InputField login;
    public TMP_InputField password;
    public Toggle check;
    public GameObject tribeChoice;
    private bool _hidden = true;

    #region Buttons

    public void Exit() => Application.Quit();

    private void Start()
    {
        var tuple = LoadCredentials();
        if (tuple.Item1 != "")
        {
            Account.Load(tuple.Item1, tuple.Item2);
            SceneManager.LoadScene("MenuScene");
        }

        AudioStatic.MenuInitSounds(this, gameObject);
    }

    public void StartRegister()
    {
        if (!IsInputCorrect())
            return;
        tribeChoice.SetActive(true);
    }

    public void FinishRegister(bool isBeaver)
    {
        if (!TryRegister())
            return;
        var tribe = isBeaver ? Tribes.Beaver : Tribes.Magpie;
        ShowSuccess("Registered successfully.");
        Connector.InitNewUser(login.text, tribe);
        tribeChoice.SetActive(false);
    }

    public void LogIn()
    {
        if (!IsInputCorrect() || !TryConnect(out var token))
            return;
        ShowSuccess("Logged in successfully.\nRedirecting.");
        if (check.isOn)
            SaveCredentials(login.text, token);
        Account.Load(login.text, token);
        SceneManager.LoadScene("MenuScene");
    }

    public void SwapPasswords()
    {
        _hidden = !_hidden;
        password.textComponent.color = new Color(0, 0, 0, _hidden ? 0 : 1);
        duckPassword.gameObject.SetActive(_hidden);
    }

    public void EnterDevMode() => SceneManager.LoadScene("DeveloperScene");

    #endregion


    private bool TryConnect(out string token)
    {
        var result = Connector.TryLogin(login.text, password.text, out var message);
        if (!result)
            ShowError(message);
        token = message;
        return result;
    }

    private void ShowSuccess(string message)
    {
        popup.ShowMessage(message, Color.green);
    }

    private bool IsInputCorrect()
    {
        var result = true;
        var sb = new StringBuilder();
        const string cyrillic = "ёйцукенгшщзхъфывапролджэячсмитьбю";
        const string special = "\\/'\"?.,()-=+*&^:%$#@!№[]{}<>";
        if (login.text.Any(s => cyrillic.Contains(char.ToLower(s)))
            || password.text.Any(s => cyrillic.Contains(char.ToLower(s)))
            || login.text.Contains(" ")
            || password.text.Contains(" ")
            || login.text.Any(s => special.Contains(s))
            || password.text.Any(s => special.Contains(s))
        )
        {
            sb.Append("Пробелы, специальные символы и кириллица запрещены!");
            result = false;
        }

        else if (login.text == string.Empty
                 || password.text == string.Empty)
        {
            sb.Append("Введите что-нибудь!");
            result = false;
        }

        if (!result)
            ShowError(sb.ToString());
        return result;
    }


    private bool TryRegister()
    {
        var result = Connector.Register(login.text, password.text);
        if (result.Contains("error"))
        {
            ShowError(result);
            return false;
        }

        ShowSuccess("Registered successfully");
        return true;
    }

    public void HideText() =>
        duckPassword.text = string
            .Join("", Enumerable.Range(0, password.text.Length)
                .Select(z => "*"));


    private void ShowError(string message)
    {
        popup.ShowMessage(message, Color.red);
    }

    public static void SaveCredentials(string login, string token)
    {
        using FileStream fs = File.Create(CredentialsPath);
        var info = new UTF8Encoding(true).GetBytes(login + " " + token);
        fs.Write(info, 0, info.Length);
    }
    
    public static void ClearCredentials()
    {
        using FileStream fs = File.Create(CredentialsPath);
        var info = new UTF8Encoding(true).GetBytes("");
        fs.Write(info, 0, info.Length);
    }

    private static Tuple<string, string> LoadCredentials()
    {
        try
        {
            using StreamReader sr = File.OpenText(CredentialsPath);
            var s = sr.ReadLine();
            if (string.IsNullOrEmpty(s))
                return Tuple.Create("", "");

            var p = s.Split(' ');

            return Tuple.Create(p[0], p[1]);
        }
        catch
        {
            return Tuple.Create("", "");
        }
    }
}