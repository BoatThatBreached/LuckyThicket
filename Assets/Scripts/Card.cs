using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class Card : MonoBehaviour
{
    public Game game;
    public Image backImage;
    public Image picture;
    public CardCharacter cardCharacter;
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
        if(game.isMyTurn)
            game.gameEngine.LoadSelfActions(cardCharacter);
    }

    public void ChangeSize(bool enlarging) =>
        transform.localScale = enlarging 
            ? new Vector3(1, 1, 1) * 1.25f 
            : new Vector3(1, 1, 1);

    public void Drag()
    {
        print("dragged!");
        //return;
        //transform.position = Input.mousePosition;
    }
    public void Drop()
    {
        print("dropped!");
        //return;
        //var p = game.pointer.Position;
        // print(p);
        // var selections = new Queue<Point>();
        // selections.Enqueue(p);
        // game.gameEngine.LoadOpponentActions(Chain, selections);
    }

    public void LoadFrom(CardCharacter cardChar, Player player)
    {
        cardCharacter = cardChar;
        game = player.game;
        Name = cardCharacter.Name;
        AbilityMask = cardCharacter.AbilityMask;
        Color = cardCharacter.Rarity switch
        {
            Rarity.Common => Color.gray,
            Rarity.Rare => Color.blue,
            Rarity.Epic => Color.magenta,
            Rarity.Legendary => (Color.red + Color.yellow) / 2,
            _ => Color.black
        };
    }
}