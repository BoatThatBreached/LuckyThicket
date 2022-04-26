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
    private Queue<Player> _turnsQueue;
    public Player currentPlayer;
    public Card currentCard;
    public GameObject cardPref;
    private Model Model { get; set; }

    public Player[] Players { get; private set; }

    private void Start()
    {
        InitPlayers();
        InitBoard();
        InitDecks();
        _turnsQueue = new Queue<Player>(Players);
        Model = new Model();
        StartTurn();
    }

    private void InitDecks()
    {
        var cards = CardCharacter.ListCards();
        //TODO: ASYNC var cardsTask = Parser.GetCardsFromFile(Enumerable.Range(0, cardsCount).Select(f=>f.ToString()));
        foreach (var p in Players)
        {
            for (var i = 0; i < 20; i++)
                p.Deck.Push(cards.GetRandom());
            for (var i = 0; i < 5; i++)
                p.DrawCard();
        }
    }

    

    private void StartTurn()
    {
        currentPlayer = _turnsQueue.Dequeue();
        currentPlayer.gameObject.SetActive(true);
    }

    public void EndTurn()
    {
        currentPlayer.gameObject.SetActive(false);
        _turnsQueue.Enqueue(currentPlayer);
        currentPlayer.Hand.Remove(currentCard);
        currentPlayer.Discard.Add(currentCard);
        Destroy(currentCard.gameObject);
        StartTurn();
    }

    private void InitBoard()
    {
        Size = 3;
        Board = new Dictionary<Point, Tile>();
        for (int i = -Size / 2; i < Size / 2 + 1; i++)
        for (int j = -Size / 2; j < Size / 2 + 1; j++)
            gameEngine.AddTile(new Point(i, j));
    }

    private void InitPlayers()
    {
        Players = FindObjectsOfType<Player>(true);

        Players[0].Name = "Maximus";
        Players[1].Name = "Michael";
        foreach (var p in Players)
        {
            p.Init();
            p.gameObject.SetActive(false);
            p.game = this;
        }


        var beaver = new Template(new[,] {{Tribes.Beaver, Tribes.Beaver, Tribes.Beaver}}, SchemaType.Big, false);
        Players[0].AddWinTemplate(beaver);
        var magpie = new Template(new[,] {{Tribes.Magpie}, {Tribes.None}, {Tribes.Magpie}}, SchemaType.Big, false);
        Players[1].AddWinTemplate(magpie);
    }
}