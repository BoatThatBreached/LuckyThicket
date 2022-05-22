﻿using System.Collections.Generic;
using System.Linq;

public static class Account
{
    public const string DeckNames = "DECK_NAMES";

    public static string Nickname;
    public static float SoundsVolume = 0.2f;
    public static float MusicVolume = 0.2f;
    public static Scenes CurrentScene;
    public static List<CardCharacter> Collection = new List<CardCharacter>();
    public static List<CardCharacter> Unowned = new List<CardCharacter>();
    public static Dictionary<string, List<int>> Decks = new Dictionary<string, List<int>>();
    public static int Balance;

    public static string Token;
    public static Room Room;
    public static string ChosenDeck;

    public static void Reset()
    {
        Nickname = string.Empty;
        Balance = 0;
        Collection = new List<CardCharacter>();
        Unowned = new List<CardCharacter>();
        Decks = new Dictionary<string, List<int>>();
        Token = string.Empty;
        Room = null;
        ChosenDeck = "";
    }

    public static void Load(string login, string token)
    {
        Nickname = login;
        Token = token;
        var maxID = Connector.GetMaxID();
        var owned = Connector.GetCollectionIDs(login);
        var ownedArray = owned.ToArray();
        var ownedCards = Connector.GetCollection(ownedArray);
        var unowned = Enumerable.Range(0, maxID + 1).Where(id => !ownedArray.Contains(id));
        var unownedCards = Connector.GetCollection(unowned);

        foreach (var card in ownedCards)
            Collection.Add(card);
        foreach (var card in unownedCards)
            Unowned.Add(card);
        GetDecks();
        if (Decks.Count == 0)
            Decks["Custom"] = new List<int>();
        if (ChosenDeck == "")
            ChosenDeck = "Custom";
        Balance = int.Parse(Connector.GetProperty("balance", login));
    }

    public static void BuyCard(CardCharacter currentChoice)
    {
        Balance -= Shop.prices[currentChoice.Rarity];
        Unowned.Remove(currentChoice);
        Collection.Add(currentChoice);
        Connector.SetProperty("balance", Balance.ToString(), Nickname);
        Connector.InitCollection(Nickname, Collection.Select(c => c.Id));
    }

    public static void SaveDecks()
    {
        foreach (var name in Decks.Keys)
            Connector.SetProperty(name, Decks[name].Select(i => i.ToString()).ToJsonList(), Nickname);
        Connector.SetProperty(DeckNames, Decks.Keys.ToJsonList(), Nickname);
        Connector.SetProperty("LAST_CHOSEN_DECK", ChosenDeck, Nickname);
    }

    private static void GetDecks()
    {
        Decks = new Dictionary<string, List<int>>();
        var names = Connector.GetProperty(DeckNames, Nickname).FromJsonList();
        foreach (var name in names)
        {
            if (name == "info not found")
                return;
            //Debug.Log(name);
            var rightName = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(name));
            var deck = Connector
                .GetProperty(rightName, Nickname);
            if (deck == "info not found")
                return;
            Decks[rightName] = deck
                .FromJsonList()
                .Select(int.Parse)
                .ToList();
        }

        ChosenDeck = Connector.GetProperty("LAST_CHOSEN_DECK", Nickname);
    }

    public static CardCharacter GetLocalCard(int index) => Connector.GetCardByID(index);
    public static CardCharacter GetGlobalCard(int index) => Connector.GetCardByID(index);

}

