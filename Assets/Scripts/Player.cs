using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using TMPro;
using Color = UnityEngine.Color;

public class Player: MonoBehaviour
{
    public TMP_Text nameField;

    public string Name
    {
        get => nameField.text;
        set => nameField.text = value;
    }

    public Game game;
    public GameObject cardPref;
    public PlayerCharacter Character;
    public Transform handPanel;
    private List<Template> CompletedTemplates { get; set; }

    public void DrawCard(int id)
    {
        var cardCharacter = Account.GetCard(id);
        var card = Instantiate(cardPref, handPanel).GetComponent<Card>();
        try
        {
            card.picture.sprite = Resources.Load<Sprite>($"cards/{cardCharacter.Name}");
        }
        catch
        {
            print("oof");
        }

        card.LoadFrom(cardCharacter, this);
    }

    public void DrawCard()
    {
        if (Character.HandList.Count >= 5)
            return;
        var id = Character.DeckList[0];
        Character.DeckList.RemoveAt(0);
        Character.HandList.Add(id);
        Character.Push();
        DrawCard(id);
    }

    public void Init()
    {
        CompletedTemplates = new List<Template>();
        Character = Account.Room.Me(Account.Nickname);
    }

    //public void AddWinTemplate(Template template) => Character.TemplatesList.Add(template);

    // важно чтобы сюда передавалась ссылка на темплэйт, находящийся в Templates - сравнение по ссылкам
    public void CompleteTemplate(Template template)
    {
        //Character.TemplatesList.Remove(template);
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
        foreach (var i in Character.TemplatesList)
            foreach (var j in board)
            {
                var posTemp = new PositionedTemplate(j.Key, i);
                if (posTemp.CheckIfMatch(board))
                    result.Add(posTemp);
            }
        
        return result;
    }  
}