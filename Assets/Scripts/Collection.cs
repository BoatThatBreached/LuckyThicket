using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collection : MonoBehaviour
{
    public GameObject CardInCollectionPref;
    public Transform CollectionPanel;
    public GameObject DeckPref;
    public Transform DecksPanel;
    public TMP_Text ActiveDeckLabel;
    public Transform ActiveDeckPanel;
    private Model Model { get; set; }

    private void Init()
    {
        Model = new Model();
        FillData();
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

        foreach (var cardCharacter in cardCharacters)
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
}