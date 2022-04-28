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
    public CardCharacter CardCharacter;
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

    public void OnMouseDown() => game.gameEngine.TryLoadActions(Chain, CardCharacter, gameObject);

    public void ChangeSize(bool enlarging) =>
        transform.localScale = enlarging 
            ? new Vector3(1, 1, 1) * 1.25f 
            : new Vector3(1, 1, 1);

    public void Drag() => transform.position = Input.mousePosition;
}