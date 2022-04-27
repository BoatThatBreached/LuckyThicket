using System;
using System.Collections;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    private string _password = string.Empty;
    public TMP_Text output;
    public void Exit() => Application.Quit();
    private int _dotsCount;
    private string _savedMessage;
    public void LogIn(TMP_InputField login)
    {
        if (!IsInputCorrect(login.text)
        // || !TryConnect(login.text)
            ) 
            return;
        ShowSuccess("Logged in successfully.\nRedirecting");
        _savedMessage = output.text;
        var cor = KekTimer(5, ShowDots, () => SceneManager.LoadScene("MenuScene"));
            
        StartCoroutine(cor);
    }

    private bool TryConnect(string login)
    {
        var result = Connector.TryLogin(login, _password, out var errors);
        if(!result)
            ShowError(errors);
        return result;
    }

    private void ShowDots()
    {
        _dotsCount++;
        _dotsCount %= 4;
        var msg = _savedMessage + string.Join("", Enumerable.Repeat(".", _dotsCount));
        output.text = msg;
    }

    private void ShowSuccess(string message)
    {
        output.text = message;
        output.color = (Color.green + Color.black)/2;
    }

    private bool IsInputCorrect(string login)
    {
        var result = true;
        var sb = new StringBuilder();
        if (login.Contains(" "))
        {
            sb.Append("Wrong login: no spaces allowed.\n");
            result = false;
        }

        if (login == string.Empty)
        {
            sb.Append("Wrong login: empty.\n");
            result = false;
        }
        if (_password.Contains(" "))
        {
            sb.Append("Wrong password: no spaces allowed.");
            result = false;
        }

        if (_password == string.Empty)
        {
            sb.Append("Wrong password: empty.");
            result = false;
        }
        if(!result)
            ShowError(sb.ToString());
        return result;
    }

    IEnumerator KekTimer(int seconds, Action process, Action finish)
    {
        var left = seconds;
        while(true)
        {
            process();
            yield return new WaitForSeconds(1);
            left -= 1;
            if (left < 0)
            {
                finish();
                yield break;
            }
        }
    }

    public void Register(TMP_InputField login)
    {
        
    }

    public void HideText(TMP_InputField pass)
    {
        var duck = string.Join("", Enumerable.Range(0, pass.text.Length).Select(z => "*"));
        if (pass.text == duck) 
            return;
        _password = pass.text;
        pass.text = duck;
    }

    private void ShowError(string message)
    {
        output.text = message;
        output.color = Color.red;
    }
    
    

    public void EnterDevMode()
    {
        SceneManager.LoadScene("DeveloperScene");
    }
}
