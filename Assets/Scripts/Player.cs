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
    public Stack<CardCharacter> Deck { get; set; }
    public List<Card> Discard { get; set; }
    public List<Card> Hand { get; set; }
    public Transform handPanel;
    private List<Template> Templates { get; set; }
    private List<Template> CompletedTemplates { get; set; }

    public void DrawCard()
    {
        var cardCharacter = Deck.Pop();
        var card = Instantiate(game.cardPref, handPanel).GetComponent<Card>();
        card.Chain = cardCharacter.Ability;
        card.game = game;
        card.Name = cardCharacter.Name;
        card.AbilityMask = cardCharacter.AbilityMask;
        switch (cardCharacter.Rarity)
        {
            case Rarity.Common:
                card.Color = Color.gray;
                break;
            case Rarity.Rare:
                card.Color = Color.blue;
                break;
            case Rarity.Epic:
                card.Color = Color.magenta;
                break;
            case Rarity.Legendary:
                card.Color = (Color.red + Color.yellow) / 2;
                break;
        }
    }

    public void Init()
    {
        Templates = new List<Template>();
        CompletedTemplates = new List<Template>();
        Deck = new Stack<CardCharacter>();
        Discard = new List<Card>();
        Hand = new List<Card>();
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