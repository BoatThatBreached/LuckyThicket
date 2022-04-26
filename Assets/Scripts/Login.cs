using System.Linq;
using TMPro;
using UnityEngine;

public class Login : MonoBehaviour
{
    private string _password = string.Empty;
    public Connector connector;
    public void Exit() => Application.Quit();

    public void LogIn(TMP_InputField login)
    {
        connector.GetCardByID(0);
        if (login.text.Length == 0)
        {
            print($"log in failed: empty login input field");
            return;
        }
        if(_password.Length==0)
        {
            print("log in failed: empty password input field");
            return;
        }

        
        //if(Connector.TryLogin(login.text, _password))
        //    print($"logged in successfully in account {login.text}:{_password}");
    }

    public void Register(TMP_InputField login)
    {
        if (login.text.Length == 0)
        {
            print($"registration failed: empty login input field");
            return;
        }
        if(_password.Length==0)
        {
            print("registration failed: empty password input field");
            return;
        }
        print($"registered successfully the account {login.text}:{_password}");
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
