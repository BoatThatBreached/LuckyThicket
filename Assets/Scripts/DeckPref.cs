using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class DeckPref : MonoBehaviour
{
    public List<int> CardsId { get; set; }
    public const int MaxCardCountInDeck = 20;
    public const int MinCardCountInDeck = 15;

    public static readonly Dictionary<Rarity, int> MaxRarity = new Dictionary<Rarity, int>()
    {
        [Rarity.Common] = Int32.MaxValue,
        [Rarity.Rare] = Int32.MaxValue,
        [Rarity.Epic] = 5,
        [Rarity.Legendary] = 1
    };

    public Image backImage;
    public TMP_Text nameField;

    public Color Color
    {
        set => backImage.color = value;
    }

    public string Name
    {
        get => nameField.text;
        set => nameField.text = value;
    }

    public void OnMouseDown()
    {
        var collection = FindObjectOfType<Collection>();
        collection.CurrentDeckName = Name;
        collection.Reload();
    }
    
    public static (bool, string) IsValid(List<CardCharacter> cardsId)
    {
        if (cardsId.Count > MaxCardCountInDeck)
            return (false, $"В колоде может быть не более {MaxCardCountInDeck} карт");
        if (cardsId.Count < MinCardCountInDeck)
            return (false, $"В колоде должно быть хотя бы {MinCardCountInDeck} карт");
        return (true, "");
    }

    public static (bool, string) CanAddCard(List<CardCharacter> cards, CardCharacter card)
    {
        if (cards.Count + 1 > MaxCardCountInDeck)
            return (false, $"В колоде может быть не более {MaxCardCountInDeck} карт");
        var cardWithSameRarity = cards.Where(c => c.Rarity == card.Rarity);
        if (cardWithSameRarity.Count() >= MaxRarity[card.Rarity])
        {
            return (false, $"Нельзя использовать больше {MaxRarity[card.Rarity]} {card.Rarity} карт");
        }
        return (true, "");
    }

    public static string GetCountLabel<T>(List<T> cards)
    {
        var count = cards.Count.ToString();
        return $"{count}/{MaxCardCountInDeck}";
    }
    
    public (bool, string) IsValid()
    {
        return IsValid(CardsId.Select(id => Account.Collection.FirstOrDefault(card => card.Id == id)).ToList());
    }
}