using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckPref : MonoBehaviour
{
    public Deck ThisDeck { get; set; }
    
    public TMP_Text nameField;
    public string Name
    {
        get => nameField.text;
        set => nameField.text = value;
    }
    
    public void OnMouseDown()
    {
        var collection = FindObjectOfType<Collection>();
        Deck.SetActiveDeck(ThisDeck);
        collection.Flush();
    }
}