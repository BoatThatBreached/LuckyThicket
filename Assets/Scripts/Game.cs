using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameObject tilePref;

    public Dictionary<Point, Tile> Board { get; private set; }
    public OccupantDesigner designer;
    //private int Size { get; set; }
    public Engine gameEngine;
    public Player player;
    public Opponent opponent;
    public CardCharacter currentCardCharacter;
    public GameObject currentCard;
    public TMP_Text turnText;
    public bool isMyTurn;
    public TMP_Text tasks;

    private void Start()
    {
        AudioStatic.GameInitSounds(this, gameObject);

        designer.Init();
        InitPlayer();
        InitOpponent();
        //InitBoard(new Point(0, 2));
        RefreshBoard(Account.Room.Board);
        InitDeck();
        StartTurn();
    }

    private void InitPlayer()
    {
        player.Init();
        player.Name = Account.Nickname;
        //var littleTemplate = Parser.GetTemplateFromString("Beaver Beaver None|None None None|None None Beaver");
        // columns from down to top.
        // rows from left to right
        // this converts to 
        // **B
        // B**
        // B**
        //player.AddWinTemplate(littleTemplate);
    }

    private void InitOpponent()
    {
        opponent.Init();
        opponent.Name = Account.Room.Other(Account.Nickname);
        // var littleTemplate = Parser.GetTemplateFromString("Beaver");
        //
        // player.AddWinTemplate(littleTemplate);
        // player.AddWinTemplate(littleTemplate);
    }

    private void InitDeck()
    {
        var cards = Account.Decks[Account.ChosenDeck];
        foreach (var index in cards)
            player.Deck.Push(Account.GetCard(index));
        for (var i = 0; i < 5; i++)
            player.DrawCard();
    }

    private void StartTurn()
    {
        currentCardCharacter = null;
        currentCard = null;
        Account.Room = Connector.GetRoomsList().Find(room => room.Name == Account.Room.Name);
        //print(Account.Room.ToJson());
        if (Account.Room == null)
        {
            Lose(false);
            return;
        }

        tasks.text =
            $"Вы выполнили {player.CompletedCount()[SchemaType.Small]}/2 малых задач и {player.CompletedCount()[SchemaType.Big]}/1 больших.";
        var lastPlayer = Account.Room.LastTurn ?? Account.Room.SecondPlayer;
        isMyTurn = lastPlayer != Account.Nickname;
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
            gameEngine.Build(p);
            if (newBoard[p] != Tribes.None)
                gameEngine.Spawn(p, newBoard[p]);
        }
    }

    public void EndTurn()
    {
        player.Hand.Remove(currentCardCharacter);
        player.Discard.Add(currentCardCharacter);
        Destroy(currentCard);
        print(Connector.SendRoom(Account.Room.Name.ToSystemRoom(), Account.Token, Parser.ConvertBoardToJson(Board)));
        StartTurn();
    }

    public void Exit() => SceneManager.LoadScene("MenuScene");

    public void Forfeit()
    {
        Connector.DestroyRoom(Account.Token, Account.Room.Name.ToSystemRoom());
        SceneManager.LoadScene("RoomScene");
    }

    public void Win(bool shouldDestroy)
    {
        print($"{player.Name} won!");
        if (shouldDestroy)
            Connector.DestroyRoom(Account.Token, Account.Room.Name.ToSystemRoom());
        SceneManager.LoadScene("RoomScene");
    }

    public void Lose(bool shouldDestroy)
    {
        print($"{player.Name} lost :(");
        if (shouldDestroy)
            Connector.DestroyRoom(Account.Token, Account.Room.Name.ToSystemRoom());
        SceneManager.LoadScene("RoomScene");
    }
}