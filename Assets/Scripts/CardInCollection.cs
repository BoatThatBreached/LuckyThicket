using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class CardInCollection : MonoBehaviour
{
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
    }

    public void ChangeSize(bool enlarging) =>
        transform.localScale = enlarging 
            ? new Vector3(1, 1, 1) * 1.25f 
            : new Vector3(1, 1, 1);

    public void Drag() => transform.position = Input.mousePosition;
}
