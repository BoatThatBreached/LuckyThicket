using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;

public class Deck : Model
{
    [SerializeField] public int Id;
    [SerializeField] public string Name;

    public Deck(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public void CreateDeck(string name)
    {
        Action<SqliteCommand> query = (command) =>
        {
            command.CommandText = @$"
INSERT INTO Decks (Name)
VALUES
('{name}')";
            command.ExecuteNonQuery();
        };
        _database.WithOpenClose(query);
    }

    public static void PlaceCardInDeck(CardCharacter card, Deck deck)
    {
        Action<SqliteCommand> query = (command) =>
        {
            command.CommandText = @$"
INSERT INTO Content (CardId, DeckId)
VALUES ({card.Id}, {deck.Id})";
            command.ExecuteNonQuery();
        };
        _database.WithOpenClose(query);
    }

    public static List<Deck> ListDecks()
    {
        Func<SqliteCommand, List<Deck>> query = (command) =>
        {
            command.CommandText = "SELECT * FROM Decks;";
            using var rdr = command.ExecuteReader();

            var res = new List<Deck>();
            while (rdr.Read())
            {
                var cc = new Deck(
                    id: rdr.GetInt32(0),
                    name: rdr.GetString(1)
                );
                res.Add(cc);
            }

            return res;
        };
        return _database.WithOpenClose(query);
    }

    public static List<CardCharacter> ListCardsInDeck(Deck deck)
    {
        Func<SqliteCommand, List<CardCharacter>> query = (command) =>
        {
            command.CommandText = @"
SELECT (c.Id, c.Name, c.Rarity, c.AbilityMask, c.AbilityString)
FROM Content content
	JOIN Decks d ON {deck.Id} == content.DeckId
	JOIN Cards c ON c.Id == content.CardId;
";
            using var rdr = command.ExecuteReader();

            var res = new List<CardCharacter>();
            while (rdr.Read())
            {
                var cc = new CardCharacter(
                    id: rdr.GetInt32(0),
                    name: rdr.GetString(1),
                    abilityMask: rdr.GetString(3),
                    ability: Serializer.AbilityFromString(rdr.GetString(4)),
                    rarity: Serializer.RarityFromString(rdr.GetString(2))
                );
                res.Add(cc);
            }

            return res;
        };
        return _database.WithOpenClose(query); 
    }
}