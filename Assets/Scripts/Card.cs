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
    public bool unplayable;

    private Color Color
    {
        set => backImage.color = value;
    }

    public TMP_Text nameField;

    private string Name
    {
        get => nameField.text;
        set => nameField.text = value;
    }

    public TMP_Text abilityField;

    private string AbilityMask
    {
        get => abilityField.text;
        set => abilityField.text = value;
    }

    public void OnMouseDown()
    {
        AudioStatic.PlayAudio("Sounds/card");
        if (!game.isMyTurn||unplayable||game.gameEngine.CurrentChain.Count>0)
            return;
        game.SelectCard(this);
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

    public void LoadFrom(CardCharacter cardChar, Player player = null)
    {
        cardCharacter = cardChar;
        if (!(player is null))
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
        try
        {
            picture.sprite = Resources.Load<Sprite>($"cards/{cardCharacter.Name}");
        }
        catch
        {
            print("oof");
            var cardName = cardChar.Name.ToLower().Contains("боб") ? "Бобрёнок" : "Сорочонок";
            picture.sprite = Resources.Load<Sprite>($"cards/{cardName}");
        }
        
    }
}