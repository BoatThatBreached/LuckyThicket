using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Collection : MonoBehaviour
{
    public GameObject collectionPanel;
    public GameObject deckPanel;
    public GameObject decksPanel;

    public GameObject deckPref;
    public GameObject cardInCollectionPref;
    public GameObject cardInDeckPref;
    public Toggle reverser;
    public TMP_InputField finder;
    public TMP_Text deckTitle;
    public TMP_Text cardCountInDeck;
    public GameObject displayErrorDialog;
    public TMP_Text displayErrorText;
    public ScrollRect CollectionScroll;
    public ScrollRect DeckScroll;
    public ScrollRect DecksScroll;
    
    public TMP_InputField newDeckName;
    public string CurrentDeckName { get; set; }
    public List<int> CurrentDeck
    {
        get => Account.Decks[CurrentDeckName];
        set => Account.Decks[CurrentDeckName] = value;
    }
    public List<CardCharacter> CardsInDeck { get; set; }

    private void Start()
    {
        Account.CurrentScene = Scenes.Collection;
        
        AudioStatic.MenuInitSounds(this, gameObject);
        
        CurrentDeckName = Account.Decks.Keys.FirstOrDefault();
        SortByRarity();
    }

    public void Reload()
    {
        Reload(Account.Collection, collectionPanel);
        CardsInDeck = CurrentDeck.Select(id => Account.Collection.FirstOrDefault(card => card.Id == id)).ToList();
        Reload(CardsInDeck, deckPanel);
        ReloadDecks();
    }

    private void Reload(IEnumerable<CardCharacter> cards, GameObject panel)
    {
        foreach (Transform child in panel.transform)
            Destroy(child.gameObject);
        var offer = reverser.isOn
            ? cards.OrderByDescending(_criteria).ToList()
            : cards.OrderBy(_criteria).ToList();
        if (finder.text.Length > 0)
            offer = offer.Where(s
                => s.Name.ToLower()
                       .Contains(finder.text.ToLower())
                   || s.AbilityMask.ToLower().Contains(finder.text.ToLower())).ToList();
        foreach (var cardGroup in offer.GroupBy(card => card))
        {
            var count = cardGroup.Count();
            var countString = count == 1 ? "" : $" ({count.ToString()})";
            var card = cardGroup.Key;
            
            if (panel == collectionPanel)
            {
                var cardChar = Instantiate(cardInCollectionPref, panel.transform).GetComponent<CardInCollection>();
                cardChar.AbilityMask = cardGroup.Key.AbilityMask;
                cardChar.Name = card.Name + countString;
                cardChar.rarity = card.Rarity;
                cardChar.Color = card.Rarity switch
                {
                    Rarity.Common => Color.gray,
                    Rarity.Rare => Color.blue,
                    Rarity.Epic => Color.magenta,
                    Rarity.Legendary => (Color.red + Color.yellow) / 2,
                    _ => Color.black
                };
                try
                {
                    cardChar.picture.sprite = Resources.Load<Sprite>($"cards/{card.Name}");
                }
                catch
                {
                    print("oof");
                }
                cardChar.CardCharacter = card;
                cardChar.scrollRect = CollectionScroll;
            }
            else if (panel == deckPanel)
            {
                var cardChar = Instantiate(cardInDeckPref, panel.transform).GetComponent<CardInDeck>();
                cardChar.Name = card.Name + countString;
                cardChar.rarity = card.Rarity;
                cardChar.Color = card.Rarity switch
                {
                    Rarity.Common => Color.gray,
                    Rarity.Rare => Color.blue,
                    Rarity.Epic => Color.magenta,
                    Rarity.Legendary => (Color.red + Color.yellow) / 2,
                    _ => Color.black
                };
                cardChar.CardCharacter = card;
                cardChar.scrollRect = DeckScroll;
            }
            
        }
    }

    private void ReloadDecks()
    {
        foreach (Transform child in decksPanel.transform)
            Destroy(child.gameObject);
        foreach (var keyValuePair in Account.Decks)
        {
            var deckComponent = Instantiate(deckPref, decksPanel.transform).GetComponent<DeckPref>();
            deckComponent.Name = keyValuePair.Key;
            deckComponent.CardsId = keyValuePair.Value;
            deckComponent.Color = deckComponent.IsValid().Item1 switch
            {
                true => new Color(20/255f, 164/255f, 0/255f),
                false => Color.red
            };
        }

        deckTitle.text = CurrentDeckName;
        Account.ChosenDeck = CurrentDeckName;
        cardCountInDeck.text = DeckPref.GetCountLabel(CurrentDeck);
        cardCountInDeck.color = DeckPref.IsValid(CardsInDeck).Item1 switch
        {
            true => new Color(20/255f, 164/255f, 0/255f),
            false => Color.red
        };
    }

    public void CreateDeck()
    {
        var deckName = newDeckName.text;
        Account.Decks[deckName] = new List<int>();
        Reload();
        newDeckName.text = "";
    }

    public void MoveCardToDeck(CardInCollection cardInCollection)
    {
        var (canAdd, errorMessage) = DeckPref.CanAddCard(CardsInDeck, cardInCollection.CardCharacter);
        if (canAdd)
            CurrentDeck.Add(cardInCollection.CardCharacter.Id);
        else
            DisplayErrorMessage(errorMessage);
        Account.SaveDecks();
        Reload();
    }
    
    public void MoveCardToCollection(CardInDeck cardInDeck)
    {
        CurrentDeck.Remove(cardInDeck.CardCharacter.Id);
        Account.SaveDecks();
        Reload();
    }

    public void DisplayErrorMessage(string message)
    {
        Debug.Log(message);
        displayErrorDialog.SetActive(true);
        displayErrorText.text = message;
    }

    public void CloseDialog()
    {
        displayErrorDialog.SetActive(false);
    }
    
    public void BackToMenu()
    {
        var (yes, errorMessage) = DeckPref.IsValid(CardsInDeck);
        if (yes)
        {
            SceneManager.LoadScene("MenuScene");
        }
        else
            DisplayErrorMessage(errorMessage);
    }

    private Func<CardCharacter, object> _criteria;

    public void SortByRarity()
    {
        _criteria = c => (int) c.Rarity;
        Reload();
    }

    public void SortByName()
    {
        _criteria = c => c.Name;
        Reload();
    }

    public void SortByComplexity()
    {
        _criteria = c => c.Ability.Count;
        Reload();
    }
}