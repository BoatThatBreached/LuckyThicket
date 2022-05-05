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
    public Toggle reverser;
    public TMP_InputField finder;
    public TMP_Text deckTitle;
    public TMP_Text cardCountInDeck;
    public GameObject displayErrorDialog;
    public TMP_Text displayErrorText;

    public TMP_InputField newDeckName;
    public string CurrentDeck { get; set; }
    public const int MaxCardCountInDeck = 20;
    public const int MinCardCountInDeck = 15;

    private void Start()
    {
        Account.CurrentScene = Scenes.Collection;
        AudioStatic.AddMainTheme(AudioStatic.MainTheme, gameObject);
        AudioStatic.AddSoundsToButtons(AudioStatic.Click, gameObject);
        
        CurrentDeck = Account.Decks.Keys.FirstOrDefault();
        SortByRarity();
    }

    public void Reload()
    {
        Reload(Account.Collection, collectionPanel);
        var cardInDeck = Account.Decks[CurrentDeck].Select(id => Account.Collection.FirstOrDefault(card => card.Id == id));
        Reload(cardInDeck, deckPanel);
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
            cardChar.CardCharacter = card;
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
        }

        deckTitle.text = CurrentDeck;
        var count = Account.Decks[CurrentDeck].Count.ToString();
        cardCountInDeck.text = $"{count}/{MaxCardCountInDeck}";
    }

    public void CreateDeck()
    {
        var deckName = newDeckName.text;
        Account.Decks[deckName] = new List<int>();
        Reload();
        newDeckName.text = "";
    }

    public (bool, string) CanSaveDeck()
    {
        var currentDeck = Account.Decks[CurrentDeck];
        if (currentDeck.Count > MaxCardCountInDeck)
            return (false, $"В колоде может быть не более {MaxCardCountInDeck} карт");
        if (currentDeck.Count < MinCardCountInDeck)
            return (false, $"В колоде должно быть хотя бы {MinCardCountInDeck} карт");
        return (true, "");
    }

    public (bool, string) CanAddCard(CardCharacter card)
    {
        var currentDeck = Account.Decks[CurrentDeck];
        if (currentDeck.Count + 1 > MaxCardCountInDeck)
            return (false, $"В колоде может быть не более {MaxCardCountInDeck} карт");
        return (true, "");
    }

    public void SwapCard(CardInCollection cardInCollection)
    {
        var parent = cardInCollection.transform.parent.gameObject;

        if (parent == collectionPanel)
        {
            var (canAdd, errorMessage) = CanAddCard(cardInCollection.CardCharacter);
            if (canAdd)
                Account.Decks[CurrentDeck].Add(cardInCollection.CardCharacter.Id);
            else
                DisplayErrorMessage(errorMessage);
        }
        else if (parent == deckPanel)
            Account.Decks[CurrentDeck].Remove(cardInCollection.CardCharacter.Id);
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
        var (yes, errorMessage) = CanSaveDeck();
        if (yes)
        {
            AudioStatic.RememberThemeState(gameObject);
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