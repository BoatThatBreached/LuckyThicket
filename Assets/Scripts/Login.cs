using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public TMP_Text duckPassword;
    public TMP_Text output;
    public TMP_InputField login;
    public TMP_InputField password;
    public GameObject TribeChoice;
    private bool _hidden = true;

    #region Buttons

    public void Exit() => Application.Quit();

    private void Start() => Account.CurrentScene = Scenes.Login;

    public void StartRegister()
    {
        if (!IsInputCorrect())
            return;
        TribeChoice.SetActive(true);
    }

    public void FinishRegister(bool isBeaver)
    {
        if (!TryRegister())
            return;
        var tribe = isBeaver ? Tribes.Beaver : Tribes.Magpie;
        ShowSuccess("Registered successfully.");
        Connector.InitNewUser(login.text, tribe);
        TribeChoice.SetActive(false);
    }

    public void LogIn()
    {
        if (!IsInputCorrect() || !TryConnect()) 
            return;
        ShowSuccess("Logged in successfully.\nRedirecting.");
        Account.Load(login.text);
        SceneManager.LoadScene("MenuScene");
    }
    
    public void SwapPasswords()
    {
        _hidden = !_hidden;
        password.textComponent.color = new Color(1, 1, 1, _hidden ? 0 : 1);
        duckPassword.gameObject.SetActive(_hidden);
    }
    
    public void EnterDevMode() => SceneManager.LoadScene("DeveloperScene");
    

    #endregion
    

    private bool TryConnect()
    {
        var result = Connector.TryLogin(login.text, password.text, out var errors);
        if (!result)
            ShowError(errors);
        return result;
    }

    private void ShowSuccess(string message)
    {
        output.text = message;
        output.color = (Color.green + Color.black) / 2;
    }

    private bool IsInputCorrect()
    {
        var result = true;
        var sb = new StringBuilder();
        const string cyrillic = "ёйцукенгшщзхъфывапролджэячсмитьбю";
        if (login.text.Any(s => cyrillic.Contains(char.ToLower(s))))
        {
            sb.Append("Wrong login: no cyrillic allowed.\n");
            result = false;
        }

        if (login.text.Contains(" "))
        {
            sb.Append("Wrong login: no spaces allowed.\n");
            result = false;
        }

        if (login.text == string.Empty)
        {
            sb.Append("Wrong login: empty.\n");
            result = false;
        }

        if (password.text.Contains(" "))
        {
            sb.Append("Wrong password: no spaces allowed.");
            result = false;
        }

        if (password.text == string.Empty)
        {
            sb.Append("Wrong password: empty.");
            result = false;
        }

        if (password.text.Any(s => cyrillic.Contains(char.ToLower(s))))
        {
            sb.Append("Wrong password: no cyrillic allowed.\n");
            result = false;
        }

        if (!result)
            ShowError(sb.ToString());
        return result;
    }


    private bool TryRegister()
    {
        var result = Connector.TryRegister(login.text, password.text, out var errors);
        if (!result)
            ShowError(errors);
        return result;
    }

    public void HideText()=> duckPassword.text = string.Join("", Enumerable.Range(0, password.text.Length).Select(z => "*"));

    

    private void ShowError(string message)
    {
        output.text = message;
        output.color = Color.red;
    }
}