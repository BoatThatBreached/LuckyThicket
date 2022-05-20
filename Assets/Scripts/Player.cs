using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using TMPro;

public class Player: MonoBehaviour
{
    public TMP_Text nameField;

    public string Name
    {
        get => nameField.text;
        set => nameField.text = value;
    }

    public bool HasWon => Character.TemplatesList.Count(t=>t.Type==SchemaType.Big)==0 
                          || Character.TemplatesList.Count(t=>t.Type==SchemaType.Small)==0;

    public int CompletedSmall => 2-Character.TemplatesList.Count(t => t.Type == SchemaType.Small);
    public int CompletedBig => 1-Character.TemplatesList.Count(t => t.Type == SchemaType.Big);

    public Game game;
    public GameObject cardPref;
    public PlayerCharacter Character;
    public Transform handPanel;

    public void DrawCard(int id)
    {
        var cardCharacter = Account.GetLocalCard(id);
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

    public void DrawCard(bool drawnByCard)
    {
        var extra = drawnByCard ? 1 : 0;
        if (Character.HandList.Count >=  extra+5)
            return;
        var id = Character.DeckList[0];
        Character.DeckList.RemoveAt(0);
        Character.HandList.Add(id);
        Character.Push();
        DrawCard(id);
    }

    public void Init()
    {
        Character = Account.Room.Me;
    }
    
    public void CompleteTemplate(Template template)
    {
        Character.TemplatesList.Remove(template);
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