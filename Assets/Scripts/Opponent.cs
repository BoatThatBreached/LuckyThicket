using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    public TMP_Text nameField;

    public string Name
    {
        get => nameField.text.Split(' ')[3];
        set => nameField.text = $"Вы играете против {value}";
    }

    private List<Template> Templates { get; set; }
    private List<Template> CompletedTemplates { get; set; }

    public void Init()
    {
        Templates = new List<Template>();
        CompletedTemplates = new List<Template>();
    }

    public void AddWinTemplate(Template template) => Templates.Add(template);

    // важно чтобы сюда передавалась ссылка на темплэйт, находящийся в Templates - сравнение по ссылкам
    public void CompleteTemplate(Template template)
    {
        Templates.Remove(template);
        CompletedTemplates.Add(template);
    }

    public Dictionary<SchemaType, int> CompletedCount()
    {
        var dict = new Dictionary<SchemaType, int>() {{SchemaType.Big, 0}, {SchemaType.Small, 0}};
        foreach (var template in CompletedTemplates)
            dict[template.Type] += 1;
        return dict;
    }

    public List<PositionedTemplate> GetTemplatesPlayerCanComplete(Dictionary<Point, Tile> board)
    {
        var result = new List<PositionedTemplate>();
        foreach (var i in Templates)
        foreach (var j in board)
        {
            var posTemp = new PositionedTemplate(j.Key, i);
            if (posTemp.CheckIfMatch(board))
                result.Add(posTemp);
        }

        return result;
    }
}