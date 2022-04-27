using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

static class OrderExtension
{
    public static IComparable OrderByName(CardCharacter card)
    {
        return card.Name;
    }

    public static IComparable OrderByRarity(CardCharacter card)
    {
        return card.Rarity;
    }
}


public class OrderMethod
{
    public Func<CardCharacter, IComparable> OrderFunction { get; set; }
    public bool IsDescending;

    public OrderMethod(Func<CardCharacter, IComparable> orderFunction, bool isDescending)
    {
        OrderFunction = orderFunction;
        IsDescending = isDescending;
    }

    public IEnumerable<CardCharacter> Order(IEnumerable<CardCharacter> cards)
    {
        return IsDescending ? cards.OrderByDescending(OrderFunction) : cards.OrderBy(OrderFunction);
    }
}

public class Collection : MonoBehaviour
{
    public GameObject CardInCollectionPref;
    public Transform CollectionPanel;
    public GameObject DeckPref;
    public Transform DecksPanel;
    public TMP_Text ActiveDeckLabel;
    public Transform ActiveDeckPanel;
    private OrderMethod OrderMethod { get; set;}
    private Model Model { get; set; }

    private void Init()
    {
        Model = new Model();
        FillData();
        OrderMethod = new OrderMethod(OrderExtension.OrderByName, true);
    }

    public void Flush()
    {
        var cardsInActiveDeck = Deck.ListCardsInDeck(Deck.GetActiveDeck());
        FlushCardsAtPanel(cardsInActiveDeck, ActiveDeckPanel);

        var otherCardsInCollection = Deck.GetCardsNotInActiveDeck();
        FlushCardsAtPanel(otherCardsInCollection, CollectionPanel);

        FlashDecks();
        ActiveDeckLabel.text = Deck.GetActiveDeck().Name;
    }

    private void Start()
    {
        Init();
        Flush();
    }

    private void FlushCardsAtPanel(IEnumerable<CardCharacter> cardCharacters, Transform panel)
    {
        foreach (var oldCard in panel.GetComponentsInChildren<CardInCollection>())
        {
            Destroy(oldCard.gameObject);
        }
        
        foreach (var cardCharacter in OrderMethod.Order(cardCharacters))
        {
            var card = Instantiate(CardInCollectionPref, panel).GetComponent<CardInCollection>();
            card.CardCharacter = cardCharacter;
            card.Chain = cardCharacter.Ability;
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
    }

    private void FlashDecks()
    {
        foreach (var oldDeck in DecksPanel.GetComponentsInChildren<DeckPref>())
        {
            Destroy(oldDeck.gameObject);
        }
        
        foreach (var deck in Deck.ListDecks())
        {
            var deckPref = Instantiate(DeckPref, DecksPanel).GetComponent<DeckPref>();
            deckPref.ThisDeck = deck;
            deckPref.Name = deck.Name;
        }
    }


    private void FillData()
    {
        CardCharacter.FillTestData();
        Deck.FillTestData();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void OnClickOrderByName()
    {
        var IsAlready = OrderMethod.OrderFunction == OrderExtension.OrderByName;
        OrderMethod = new OrderMethod(
            OrderExtension.OrderByName,
            IsAlready && !OrderMethod.IsDescending);
        Flush();
    }
    
    public void OnClickOrderByRarity()
    {
        var IsAlready = OrderMethod.OrderFunction == OrderExtension.OrderByRarity;
        OrderMethod = new OrderMethod(
            OrderExtension.OrderByRarity,
            IsAlready && !OrderMethod.IsDescending);
        Flush();
    }
    
}