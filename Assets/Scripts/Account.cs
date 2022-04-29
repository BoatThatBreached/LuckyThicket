using System;
using System.Collections.Generic;
using System.Linq;

public static class Account
{
    public  static string Nickname;
    public static float SoundsVolume = 0.25F;
    public static float MusicVolume = 0.25F;
    public static Scenes CurrentScene;
    public static List<CardCharacter> Collection = new List<CardCharacter>();
    public static List<CardCharacter> Unowned = new List<CardCharacter>();
    public static List<List<int>> Decks = new List<List<int>>();
    public static int Balance;

    public static void Reset()
    {
        Nickname = string.Empty;
        SoundsVolume = 0.5F;
        MusicVolume = 0.5F;
        Balance = 0;
        Collection = new List<CardCharacter>();
        Unowned = new List<CardCharacter>();
        Decks = new List<List<int>>();
    }

    public static void Load(string login)
    {
        Nickname = login;
        
        var maxID = Connector.GetMaxID();
        var owned = Connector.GetCollectionIDs(login);
        var ownedCards = Connector.GetCollection(owned);
        
        var unowned = Enumerable.Range(0, maxID + 1).Where(id => !owned.Contains(id));
        var unownedCards = Connector.GetCollection(unowned);
        
        foreach (var card in ownedCards)
            Collection.Add(card);
        foreach (var card in unownedCards)
            Unowned.Add(card);

        Balance = int.Parse(Connector.GetProperty("balance", login));

    }

    public static void BuyCard(CardCharacter currentChoice)
    {
        Balance -= Shop.prices[currentChoice.Rarity];
        Unowned.Remove(currentChoice);
        Collection.Add(currentChoice);
        Connector.SetProperty("balance", Balance.ToString(), Nickname);
        Connector.InitCollection(Nickname, Collection.Select(c=>c.Id));
    }
}