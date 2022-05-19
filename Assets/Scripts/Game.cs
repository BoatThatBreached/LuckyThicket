using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameObject tilePref;

    public Dictionary<Point, Tile> Board { get; private set; }
    public OccupantDesigner designer;
    public Engine gameEngine;
    public Player player;
    public Opponent opponent;
    public TMP_Text turnText;
    public bool isMyTurn;
    public TMP_Text tasks;

    private void Start()
    {
        AudioStatic.GameInitSounds(this, gameObject);

        designer.Init();
        InitPlayer();
        InitBoard();
        InitOpponent();
        
        StartTurn();
    }

    private void InitBoard()
    {
        var board = Parser.EmptyBoard(5, new Point(0,2));
        Board = new Dictionary<Point, Tile>();
        foreach (var point in board.Keys)
            gameEngine.Build(point);
        // foreach (var note in Account.Room.Data.LogList)
        //     ApplyOtherPlayerTurn(note);
    }

    private void InitPlayer()
    {
        player.Init();
        player.Name = Account.Nickname;
        //var littleTemplate = Parser.GetTemplateFromString("Beaver Beaver None|None None None|None None Beaver");
        //player.AddWinTemplate(littleTemplate);
    }

    private void InitOpponent()
    {
        opponent.Init();
        opponent.Name = Account.Room.Other(Account.Nickname).Login;
    }

    private void InitCards()
    {
        foreach(Transform child in player.handPanel)
            Destroy(child.gameObject);
        foreach (var id in player.Character.HandList)
            player.DrawCard(id);
    }

    private void StartTurn()
    {
        var room = Connector.GetRoomsList().Find(room => room.Name == Account.Room.Name);
        if (room == null)
        {
            Win(false);
            return;
        }
        room.Data.FirstPlayer.Pull();
        room.Data.SecondPlayer.Pull();
        print(room.Data.Log);
        Account.Room = room;
        //Account.Room.Push();
        
        InitCards();
        tasks.text =
            $"Вы выполнили {player.CompletedCount()[SchemaType.Small]}/2 малых задач и {player.CompletedCount()[SchemaType.Big]}/1 больших.";
        var lastPlayer = Account.Room.LastTurn ?? Account.Room.Data.SecondPlayer.Login;
        print(lastPlayer);
        isMyTurn = lastPlayer != Account.Nickname;
        turnText.text = isMyTurn ? "Your turn!" : "Opponent's turn!";

        //RefreshBoard(Account.Room.Board);
        if (isMyTurn)
        {
            if(Account.Room.Data.LogList.Count>0)
                ApplyOtherPlayerTurn(Account.Room.Data.LogList.Last());
            return;
        }
        print("fetching!");
        var cor = Waiters.LoopFor(1.2f, StartTurn);
        StartCoroutine(cor);
    }

    private void ApplyOtherPlayerTurn(LogNote note)
    {
        gameEngine.LoadOpponentActions(
            Account.GetGlobalCard(int.Parse(note.CardID)), 
            Parser.ParseSelections(note.Selections));
    }

    public void EndTurn(CardCharacter card)
    {
        var destroyedCard = FindObjectsOfType<Card>().First(c => c.cardCharacter == card);
        Destroy(destroyedCard);
        player.Character.HandList.Remove(card.Id);
        player.Character.GraveList.Add(card.Id);
        player.Character.Push();
        var note = new LogNote(player.Character.Login, card, gameEngine.SelfSelections);
        Account.Room.Data.LogList.Add(note);
        print(Account.Room.Data.Log);
        Account.Room.Push();
        print(Connector.SendRoom(Account.Room.Name.ToSystemRoom(), Account.Token, Account.Room.DataString));
        
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