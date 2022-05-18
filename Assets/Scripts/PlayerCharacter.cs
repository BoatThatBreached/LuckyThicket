using System.Collections.Generic;
using System.Linq;

public class PlayerCharacter
{
    public string Login;
    public string Hand;
    public string Deck;
    public string Graveyard;
    public string Templates;

    public List<int> DeckList { get; set; }
    public List<int> HandList { get; set; }
    public List<int> GraveList { get; set; }
    public List<Template> TemplatesList { get; set; }

    public PlayerCharacter()
    {
        TemplatesList = new List<Template>();
    }

    public void Init()
    {
        Login = Account.Nickname;
        var deck = Account.Decks[Account.ChosenDeck].Select(Account.GetCard).Shuffled().Select(card=>card.Id).ToList();
        var hand = new List<int>();
        for(var i = 0; i<5;i++)
        {
            hand.Add(deck[0]);
            deck.RemoveAt(0);
        }
        Hand = $"[{string.Join(",", hand)}]";
        Deck = $"[{string.Join(",", deck)}]";
        Graveyard = "[]";
        Templates = "[]";
    }

    public void Pull()
    {
        DeckList = Deck.FromJsonList().Select(int.Parse).ToList();
        HandList = Hand.FromJsonList().Select(int.Parse).ToList();
        GraveList = Graveyard.FromJsonList().Select(int.Parse).ToList();
    }

    public void Push()
    {
        Deck = DeckList.ToJsonList();
        Hand = Hand.ToJsonList();
        Graveyard = GraveList.ToJsonList();
    }

}