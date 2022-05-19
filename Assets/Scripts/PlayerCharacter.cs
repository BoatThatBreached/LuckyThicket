using System;
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
        DeckList = Account.Decks[Account.ChosenDeck].Select(Account.GetLocalCard).Shuffled().Select(card => card.Id)
            .ToList();
        var fractions = Account.Decks[Account.ChosenDeck].Select(Account.GetLocalCard)
            .Select(card => card.Name.ToLower()).ToList();
        var fractionsCount = new Dictionary<Tribes, int>()
        {
            [Tribes.Beaver] = fractions.Count(s => s.Contains("бобёр") || s.Contains("бобр")),
            [Tribes.Magpie] = fractions.Count(s => s.Contains("сорок") || s.Contains("сороч"))
        };
        var maxFrac = fractionsCount.Keys.First(k => fractionsCount[k] == fractionsCount.Values.Max());
        switch (maxFrac)
        {
            case Tribes.Beaver:
                var smallBeaverTemplate1 =
                    Parser.GetTemplateFromString("None None Beaver|None Beaver None|Beaver None None");
                var bigBeaverTemplate =
                    Parser.GetTemplateFromString("None Beaver None|Beaver Beaver Beaver|None Beaver None", true);
                var smallBeaverTemplate2 =
                    Parser.GetTemplateFromString("Beaver Beaver|Beaver None");
                TemplatesList.Add(smallBeaverTemplate1);
                TemplatesList.Add(smallBeaverTemplate2);
                TemplatesList.Add(bigBeaverTemplate);
                break;
            case Tribes.Magpie:
                var smallMagpieTemplate1 =
                    Parser.GetTemplateFromString("Magpie None None|None None Magpie|Magpie None None");
                var bigMagpieTemplate =
                    Parser.GetTemplateFromString("Magpie None Magpie|None Magpie None|Magpie None Magpie", true);
                var smallMagpieTemplate2 =
                    Parser.GetTemplateFromString("None Magpie|Magpie None|None Magpie");
                TemplatesList.Add(smallMagpieTemplate1);
                TemplatesList.Add(smallMagpieTemplate2);
                TemplatesList.Add(bigMagpieTemplate);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        HandList = new List<int>();
        for (var i = 0; i < 5; i++)
        {
            HandList.Add(DeckList[0]);
            DeckList.RemoveAt(0);
        }

        GraveList = new List<int>();

        //Hand = $"[{string.Join(",", hand)}]";
        //Deck = $"[{string.Join(",", deck)}]";
        //Graveyard = "[]";
        //Templates = $"[{string.Join(",",TemplatesList.Select(t=>t.TemplateString))}]";
        Push();
    }

    public void Pull()
    {
        DeckList = Deck.FromJsonList().Select(int.Parse).ToList();
        HandList = Hand.FromJsonList().Select(int.Parse).ToList();
        GraveList = Graveyard.FromJsonList().Select(int.Parse).ToList();
        TemplatesList = Templates.FromJsonList().Select(Template.CreateFromString).ToList();
    }

    public void Push()
    {
        Deck = DeckList.ToJsonList();
        Hand = Hand.ToJsonList();
        Graveyard = GraveList.ToJsonList();
        Templates = TemplatesList.Select(t => t.TemplateString).ToJsonList();
    }
}