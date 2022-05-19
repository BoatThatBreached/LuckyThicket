using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using System.Linq;

public class Opponent : MonoBehaviour
{
    public TMP_Text nameField;
    private PlayerCharacter _character;

    public string Name
    {
        get => nameField.text.Split(' ')[3];
        set => nameField.text = $"Вы играете против {value}";
    }
    public bool HasWon => _character.TemplatesList.Count(t=>t.Type==SchemaType.Big)==0 
                          || _character.TemplatesList.Count(t=>t.Type==SchemaType.Small)==0;
    
    public void Init()
    {
        _character = Account.Room.Other;
    }
    public void CompleteTemplate(Template template)
    {
        _character.TemplatesList.Remove(template);
    }

    public List<PositionedTemplate> GetTemplatesPlayerCanComplete(Dictionary<Point, Tile> board)
    {
        var result = new List<PositionedTemplate>();
        foreach (var i in _character.TemplatesList)
        foreach (var j in board)
        {
            var posTemp = new PositionedTemplate(j.Key, i);
            if (posTemp.CheckIfMatch(board))
                result.Add(posTemp);
        }

        return result;
    }
}