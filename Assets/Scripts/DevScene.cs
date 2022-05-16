using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DevScene : MonoBehaviour
{
    public TMP_InputField cardName;
    public TMP_InputField cardMask;
    public AbRarContainer abRar;

    public void Start()
    {
        AudioStatic.MenuInitSounds(this, gameObject);
        print(Connector.GetCardByID(18));
    }

    public void Exit()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void TrySave()
    {
        var q = new Queue<Basis>(abRar.ability);
        var card = new CardCharacter(cardName.text, cardMask.text, q, abRar.rarity);
        cardName.text = string.Empty;
        cardMask.text = string.Empty;
        abRar.ability = Array.Empty<Basis>();
        ConvertCardToFile(card);
        print("Saved successfully!");
    }

    private static void ConvertCardToFile(CardCharacter card)
    {
        var sb = new StringBuilder();
        foreach (var token in card.Ability)
            sb.Append(token + " ");
        card.AbilityString = sb.ToString();
        card.Id = Connector.GetMaxID()+1;
        var ans = Connector.SendCard(card);
        print(ans);
    }

}
