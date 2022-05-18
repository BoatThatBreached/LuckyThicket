using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class LogNote
{
    public string PlayerLogin;
    public string CardID;
    public string Selections;

    public LogNote(string login, CardCharacter card, Queue<Point> selections)
    {
        PlayerLogin = login;
        CardID = card.Id.ToString();
        Selections = Parser.ConvertSelections(selections);
    }

    public override string ToString() => JsonUtility.ToJson(this);
}