using System.Collections;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    private string _password = string.Empty;
    public TMP_Text output;
    public void Exit() => Application.Quit();

    public void LogIn(TMP_InputField login)
    {
        if (login.text.Length == 0)
        {
            output.text = "log in failed: empty login input field";
            return;
        }
        if(_password.Length==0)
        {
            output.text = "log in failed: empty password input field";
            return;
        }

        if(Connector.TryLogin(login.text, _password))
        {
            output.text = $"logged in successfully in account {login.text}\nRedirecting...";
            var cor = KekTimer(5);
            StartCoroutine(cor);
            SceneManager.LoadScene("MenuScene");
        }
    }

    IEnumerator KekTimer(int seconds)
    {
        var left = seconds;
        while(true)
        {
            print(left);
            yield return new WaitForSeconds(1);
            left -= 1;
            if (left < 0)
                yield break;
        }
    }

    public void Register(TMP_InputField login)
    {
        if (login.text.Length == 0)
        {
            output.text = "registration failed: empty login input field";
            return;
        }
        if(_password.Length==0)
        {
            output.text = "registration failed: empty password input field";
            return;
        }
        if (Connector.TryRegister(login.text, _password))
            print($"registered successfully the account {login.text.ToUpper()}");
    }

    public void HideText(TMP_InputField pass)
    {
        var duck = string.Join("", Enumerable.Range(0, pass.text.Length).Select(z => "*"));
        if (pass.text == duck) 
            return;
        _password = pass.text;
        pass.text = duck;
    }
}
