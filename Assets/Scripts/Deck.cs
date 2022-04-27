using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;

public class Deck : Model
{
    [SerializeField] public readonly int Id;
    [SerializeField] public string Name;
    [SerializeField] public bool IsActive;

    public Deck(int id, string name, bool isActive)
    {
        Id = id;
        Name = name;
        IsActive = isActive;
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
    
    public static void RemoveCardFromDeck(CardCharacter card, Deck deck)
    {
        Action<SqliteCommand> query = (command) =>
        {
            command.CommandText = @$"
DELETE FROM Content
WHERE CardId == {card.Id} and DeckId == {deck.Id}";
            command.ExecuteNonQuery();
        };
        _database.WithOpenClose(query);
    }

    public static Deck GetActiveDeck()
    {
        Func<SqliteCommand, Deck> query = (command) =>
        {
            command.CommandText = @"
SELECT Id, Name, IsActive
FROM Decks
WHERE IsActive
LIMIT 1";
            using var rdr = command.ExecuteReader();

            Deck d = null;
            while (rdr.Read())
            {
                d = new Deck(
                    id: rdr.GetInt32(0),
                    name: rdr.GetString(1),
                    isActive: rdr.GetBoolean(2)
                );
            }
            return d;
        };
        var activeDeck = _database.WithOpenClose(query);
        if (activeDeck is null)
        {
            AddActiveDeck();
            return GetActiveDeck();
        }

        return activeDeck;
    }

    public static void SetActiveDeck(Deck deck)
    {

        Action<SqliteCommand> query = (command) =>
        {
            command.CommandText = @$"
UPDATE Decks
SET IsActive = FALSE;

UPDATE Decks
SET IsActive = TRUE
WHERE Id == {deck.Id};
";
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
                    name: rdr.GetString(1),
                    isActive: rdr.GetBoolean(2)
                );
                res.Add(cc);
            }

            return res;
        };
        return _database.WithOpenClose(query);
    }

    public static void DeleteFromOrAddInActiveDeck(CardCharacter card)
    {
        var activeDeck = GetActiveDeck();
        Debug.Log($"check {card.Name}");
        var cards = ListCardsInDeck(activeDeck);
        foreach (var cardCharacter in cards)
        {
            Debug.Log($"active: {cardCharacter.Name}");
        }
        if (cards.Contains(card))
        {
            Debug.Log("enter if");
            RemoveCardFromDeck(card, activeDeck);
            return;
        }
        PlaceCardInDeck(card, activeDeck);
    }

    public static List<CardCharacter> GetCardsNotInActiveDeck()
    {
        var activeDeck = GetActiveDeck();
        Func<SqliteCommand, List<CardCharacter>> query = (command) =>
        {
            command.CommandText = $@"
SELECT c.Id, c.Name, c.Rarity, c.AbilityMask, c.AbilityString
    FROM Cards c
LEFT JOIN Content content ON c.Id == content.CardId
WHERE content.DeckId is NULL or content.DeckId != {activeDeck.Id}
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
    
    public static List<CardCharacter> ListCardsInDeck(Deck deck)
    {
        Func<SqliteCommand, List<CardCharacter>> query = (command) =>
        {
            command.CommandText = $@"
SELECT c.Id, c.Name, c.Rarity, c.AbilityMask, c.AbilityString
FROM Content content
	JOIN Cards c ON c.Id == content.CardId
WHERE {deck.Id} == content.DeckId
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

    public static void AddActiveDeck()
    {
        Action<SqliteCommand> query = (command) =>
        {
            Debug.Log("Add test data");
            command.CommandText = @"
INSERT INTO Decks (Name, IsActive)
VALUES
('Cool deck', TRUE)
";
            command.ExecuteNonQuery();
        };
        _database.WithOpenClose(query);
    }

    public static void FillTestData()
    {
        if (ListDecks().Count != 0)
            return;
        Action<SqliteCommand> query = (command) =>
        {
            Debug.Log("Add test data");
            command.CommandText = @"
INSERT INTO Decks (Id, Name, IsActive)
VALUES
(0, 'Cool deck', TRUE),
(1, 'Bebrus', FAlSE),
(2, 'Magipe', FALSE);

INSERT INTO Content (CardId, DeckId)
VALUES
(0, 0),
(1, 0),
(2, 0),
(3, 0);
";
            command.ExecuteNonQuery();
        };
        _database.WithOpenClose(query);
    }
    
    
    public override bool Equals(object obj)
    {
        if (!(obj is Deck)) return false;
        var deck = obj as Deck;
        return Id == deck.Id;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}