using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public TMP_Text balance;
    public GameObject panel;
    public GameObject cardPref;
    public Toggle reverser;
    public TMP_InputField finder;
    public GameObject confirmation;
    public TMP_Text confirmationText;
    public CardCharacter currentChoice;
    public ScrollRect yourScrollRect;
    public static Dictionary<Rarity, int> prices = new Dictionary<Rarity, int>
    {
        [Rarity.Common] = 10,
        [Rarity.Rare] = 20,
        [Rarity.Epic] = 40,
        [Rarity.Legendary] = 100
    };
    //TODO: (do not) show cards that can't be bought
    private void Start()
    {
        Account.CurrentScene = Scenes.Shop;
        SortByRarity();
        Reload();
        AudioStatic.MenuInitSounds(this, gameObject);
    }

    public void Exit() => SceneManager.LoadScene("MenuScene");
    

    public void Reload()
    {
        balance.text = $"You have:\n{Account.Balance}$";
        foreach(Transform child in panel.transform)
            Destroy(child.gameObject);
        var offer = reverser.isOn 
            ? Account.Unowned.OrderByDescending(_criteria).ToList() 
            : Account.Unowned.OrderBy(_criteria).ToList();
        if (finder.text.Length > 0)
            offer = offer.Where(s 
                => s.Name.ToLower()
                    .Contains(finder.text.ToLower()) 
                   || s.AbilityMask.ToLower().Contains(finder.text.ToLower())).ToList();
        foreach (var card in offer)
        {
            var cardChar = Instantiate(cardPref, panel.transform).GetComponent<CardInCollection>();
            try
            {
                cardChar.picture.sprite = Resources.Load<Sprite>($"cards/{card.Name}");
            }
            catch
            {
                print("oof");
            }
            cardChar.AbilityMask = card.AbilityMask;
            cardChar.Name = card.Name;
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
            cardChar.scrollRect = yourScrollRect;
        }
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

    public void ShowConfirmation(CardCharacter card)
    {
        if (prices[card.Rarity] > Account.Balance)
            return;
        currentChoice = card;
        confirmation.SetActive(true);
        confirmationText.text = $"Вы уверены, что хотите купить карту \"{card.Name}\" за {prices[card.Rarity]}$?";
    }

    public void Buy()
    {
        Account.BuyCard(currentChoice);
        confirmation.SetActive(false);
        Reload();
    }



}
