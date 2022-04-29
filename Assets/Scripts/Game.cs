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
        InitBoard(new Point(0, 2));
        
        InitDeck();
        StartTurn();
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
        var data = Regex.Split(Connector.GetBoard(Account.Room.Name, Account.Token), "lastTurn");
        var board = data[0];
        //print(board);
        var lastPlayer = data[1].Substring(3);
        lastPlayer = lastPlayer.Substring(0, lastPlayer.Length - 2);
        isMyTurn = lastPlayer != Account.Nickname;
        turnText.text = isMyTurn ? "Your turn!" : "Opponent's turn!";
        var index = board.IndexOf(':');
        var json = board.Contains("null")? ""
            : board.Substring( index+ 1);
        if (json != "")
            json = json.Substring(0, json.Length - 2);
        print(json);
        if(json!="")
            RefreshBoard(Parser.ConvertJsonToBoard(json));
        if (isMyTurn) 
            return;
        print("fetching!");
        var cor = Waiters.LoopFor(5, StartTurn);
        StartCoroutine(cor);
    }

    private void RefreshBoard(Dictionary<Point, Tribes> newBoard)
    {
        var deleted = Board.Keys
            .Where(p => !newBoard.ContainsKey(p))
            .ToList();
        var added = newBoard.Keys
            .Where(p => !Board.ContainsKey(p))
            .ToList();
        var changed = Board.Keys
            .Where(p => newBoard.ContainsKey(p) && newBoard[p] != Board[p].occupantTribe)
            .ToList();
        foreach (var point in deleted)
            gameEngine.DestroyTile(point);
        foreach (var point in added)
        {
            gameEngine.AddTile(point);
            if(newBoard[point]!=Tribes.None)
                gameEngine.SpawnUnit(point, newBoard[point]);
        }
        foreach (var point in changed)
        {
            if(Board[point].occupantTribe!=Tribes.None)
                gameEngine.KillUnit(point);
            gameEngine.SpawnUnit(point, newBoard[point]);
        }
    }

    public void EndTurn()
    {
        player.Hand.Remove(currentCardCharacter);
        player.Discard.Add(currentCardCharacter);
        Destroy(currentCard);
        print(Connector.TrySendBoard(Account.Room.Name, Account.Token, Parser.ConvertBoardToJson(Board)));
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
}