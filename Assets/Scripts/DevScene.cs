using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DevScene : MonoBehaviour
{
    private string _input;
    
    public TMP_InputField password;
    public TMP_InputField cardName;
    public TMP_InputField cardMask;
    public AbRarContainer abRar;

    public void Start()
    {
        CardCharacter.SetCount(Parser.GetCardsCount());
    }

    public void Exit()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void HideText()
    {
        _input = password.text;
        var duck = "";
        for (var i = 0; i < _input.Length; i++)
            duck += "*";
        password.text = duck;
    }

    public void TrySave()
    {
        if (Hash128.Compute(_input).ToString() != "d0ccaca4712828d93a7cd3368f72308c")
            return;
        var q = new Queue<Basis>(abRar.ability);
        var card = new CardCharacter(cardName.text, cardMask.text, q, abRar.rarity);
        cardName.text = string.Empty;
        cardMask.text = string.Empty;
        abRar.ability = Array.Empty<Basis>();
        ConvertCardToFile(card);
        //print("Saved successfully!");
    }

    static void ConvertCardToFile(CardCharacter card)
    {
        var parser = new Parser();
        var data = new List<CardCharacter> {card};
        parser.ConvertToFile(data);
    }

}
