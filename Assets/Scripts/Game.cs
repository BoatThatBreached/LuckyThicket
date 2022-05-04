using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public TMP_Text turnText;
    public bool isMyTurn;
    private void Start()
    {
        designer.Init();
        InitPlayer();
        //InitBoard(new Point(0, 2));
        
        InitDeck();
        StartTurn();
        AudioStatic.AddMainTheme(AudioStatic.MainTheme, gameObject);
        AudioStatic.AddSoundsToButtons(AudioStatic.Click, gameObject);
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
        var lastPlayer = Account.Room.LastTurn;
        isMyTurn = lastPlayer == Account.Nickname;
        turnText.text = isMyTurn ? "Your turn!" : "Opponent's turn!";
        RefreshBoard(Account.Room.Board);
        if (isMyTurn) 
            return;
        print("fetching!");
        var cor = Waiters.LoopFor(2, StartTurn);
        StartCoroutine(cor);
    }

    private void RefreshBoard(Dictionary<Point, Tribes> newBoard)
    {
        Board = new Dictionary<Point, Tile>();
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        foreach (var p in newBoard.Keys)
        {
            gameEngine.AddTile(p);
            if (newBoard[p]!=Tribes.None)
                gameEngine.SpawnUnit(p, newBoard[p]);
        }
    }

    public void EndTurn()
    {
        player.Hand.Remove(currentCardCharacter);
        player.Discard.Add(currentCardCharacter);
        Destroy(currentCard);
        print(Connector.SendRoom(Account.Room.Name, Account.Token, Parser.ConvertBoardToJson(Board)));
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

    public void Exit() => SceneManager.LoadScene("MenuScene");

    public void Forfeit()
    {
        Connector.DestroyRoom(Account.Token, Account.Room.Name);
        SceneManager.LoadScene("RoomScene");
    }
}