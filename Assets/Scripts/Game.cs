using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject tilePref;

    public Dictionary<Point, Tile> Board { get; private set; }
    public OccupantDesigner designer;
    private int Size { get; set; }
    public Engine gameEngine;

    public Player player;

    public CardCharacter currentCardCharacter;

    public GameObject currentCard;

    private void Start()
    {
        InitPlayer();
        InitBoard(new Point(0, 2));
        InitDeck();
        StartTurn();
        AudioStatic.AddSoundsToButtons("button_sound", gameObject);
    }

    private void InitPlayer()
    {
        player.Init();
        player.Name = Account.Nickname;
    }

    private void InitDeck()
    {
        var cards = Account.Collection;
        for (var i = 0; i < 20; i++)
            player.Deck.Push(cards.GetRandom());
        for (var i = 0; i < 5; i++)
            player.DrawCard();
    }

    private void StartTurn()
    {
        currentCardCharacter = null;
        currentCard = null;
    }

    public void EndTurn()
    {
        player.Hand.Remove(currentCardCharacter);
        player.Discard.Add(currentCardCharacter);
        Destroy(currentCard);
        StartTurn();
    }

    private void InitBoard(Point center)
    {
        Size = 3;
        Board = new Dictionary<Point, Tile>();
        for (var i = -Size / 2; i < Size / 2 + 1; i++)
        for (var j = -Size / 2; j < Size / 2 + 1; j++)
            gameEngine.AddTile(new Point(i+center.X, j+center.Y));
    }
}