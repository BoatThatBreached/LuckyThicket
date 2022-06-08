using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class LogNote
{
    public string PlayerLogin;
    public string CardID;
    public string Selections;
    public string CompletedTemplate;

    public LogNote(string login, CardCharacter card, Queue<Point> selections, List<PositionedTemplate> templates)
    {
        PlayerLogin = login;
        CardID = card.Id.ToString();
        Selections = Parser.ConvertSelections(selections);
        CompletedTemplate = templates.Count==0 
            ? "" 
            :templates.Select(t=> Parser.CompletedTemplate(t.StartingPoint, t.Template)).ToJsonList();
    }

    public override string ToString() => JsonUtility.ToJson(this);
}