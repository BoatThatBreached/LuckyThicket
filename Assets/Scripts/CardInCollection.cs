using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class CardInCollection : MonoBehaviour
{
    public Image backImage;
    public Image picture;
    public Rarity rarity;
    public CardCharacter CardCharacter { get; set; }

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

    public void Click()
    {
        switch (Account.CurrentScene)
        {
            case Scenes.Shop:
                var shop = FindObjectOfType<Shop>();
                shop.ShowConfirmation(CardCharacter);
                break;
            case Scenes.Collection:
                var collection = FindObjectOfType<Collection>();
                collection.SwapCard(this);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ChangeSize(bool enlarging) =>
        transform.localScale = enlarging 
            ? new Vector3(1, 1, 1) * 1.25f 
            : new Vector3(1, 1, 1);

    public void Drag() => transform.position = Input.mousePosition;
}
