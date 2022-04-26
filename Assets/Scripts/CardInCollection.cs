using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class CardInCollection : MonoBehaviour
{
    public CardCharacter CardCharacter { get; set; }
    public Queue<Basis> Chain;

    public Image backImage;
    public Color Color
    {
        set => backImage.color = value;
    }
    public TMP_Text nameField;
    public string Name
    {
        get => nameField.text;
        set => nameField.text = value;
    }
    public TMP_Text abilityField;
    public string AbilityMask
    {
        get => abilityField.text;
        set => abilityField.text = value;
    } 

    public void OnMouseDown()
    {
        var deck = Deck.ListDecks().First();
        Deck.PlaceCardInDeck(CardCharacter, deck);
    }

    public void ChangeSize(bool enlarging) =>
        transform.localScale = enlarging 
            ? new Vector3(1, 1, 1) * 1.25f 
            : new Vector3(1, 1, 1);

    public void Drag() => transform.position = Input.mousePosition;
}
