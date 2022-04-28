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
    public Stack<CardCharacter> Deck { get; set; }
    public List<CardCharacter> Discard { get; set; }
    public List<CardCharacter> Hand { get; set; }
    public Transform handPanel;
    private List<Template> Templates { get; set; }
    private List<Template> CompletedTemplates { get; set; }

    public void DrawCard()
    {
        if (Hand.Count >= 5)
            return;
        var cardCharacter = Deck.Pop();
        Hand.Add(cardCharacter);
        var card = Instantiate(cardPref, handPanel).GetComponent<Card>();
        card.Chain = cardCharacter.Ability;
        card.game = game;
        card.Name = cardCharacter.Name;
        card.AbilityMask = cardCharacter.AbilityMask;
        card.Color = cardCharacter.Rarity switch
        {
            Rarity.Common => Color.gray,
            Rarity.Rare => Color.blue,
            Rarity.Epic => Color.magenta,
            Rarity.Legendary => (Color.red + Color.yellow) / 2,
            _ => Color.black
        };
        
    }

    public void Init()
    {
        Templates = new List<Template>();
        CompletedTemplates = new List<Template>();
        Deck = new Stack<CardCharacter>();
        Discard = new List<CardCharacter>();
        Hand = new List<CardCharacter>();
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