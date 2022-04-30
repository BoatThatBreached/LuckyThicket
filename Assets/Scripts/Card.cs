using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class Card : MonoBehaviour
{
    public Queue<Basis> Chain;
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
            game.gameEngine.TryLoadActions(Chain, cardCharacter, gameObject);
    }

    public void ChangeSize(bool enlarging) =>
        transform.localScale = enlarging 
            ? new Vector3(1, 1, 1) * 1.25f 
            : new Vector3(1, 1, 1);

    public void Drag()
    {
        return;
        transform.position = Input.mousePosition;
    }
    public void Drop()
    {
        return;
        //var p = game.pointer.Position;
        // print(p);
        // var selections = new Queue<Point>();
        // selections.Enqueue(p);
        // game.gameEngine.LoadActionsWithSelections_Unsafe(Chain, selections);
    }
}