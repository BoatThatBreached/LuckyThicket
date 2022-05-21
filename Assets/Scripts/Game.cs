using System.Collections;
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
    public Transform cardSlot;
    public Camera cam;
    private void Start()
    {
        AudioStatic.GameInitSounds(this, gameObject);

        designer.Init();
        InitPlayer();
        InitOpponent();
        StartCoroutine(InitBoard());


        
    }

    private IEnumerator InitBoard()
    {
        var board = Parser.EmptyBoard(5, new Point(0, 2), true);
        Board = new Dictionary<Point, Tile>();
        foreach (var point in board.Keys)
        {
            gameEngine.Build(point);
            yield return new WaitForSeconds(0.1f);
        }
        StartTurn();
    }

    private void InitPlayer()
    {
        player.Init();
        player.Name = Account.Nickname;
    }

    private void InitOpponent()
    {
        opponent.Init();
        opponent.Name = Account.Room.Other.Login;
    }

    private void InitCards()
    {
        foreach (Transform child in player.handPanel)
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
            $"Вы выполнили {player.CompletedSmall}/2 малых задач и {player.CompletedBig}/1 больших.";
        var lastPlayer = Account.Room.LastTurn ?? Account.Room.Data.SecondPlayer.Login;
        isMyTurn = lastPlayer != Account.Nickname;
        turnText.text = isMyTurn ? "Your turn!" : "Opponent's turn!";

        if (isMyTurn)
        {
            if (Account.Room.Data.LogList.Count > 0)
                StartCoroutine(ApplyingTurn(Account.Room.Data.LogList.Last()));
            return;
        }

        print("fetching!");
        var cor = Waiters.LoopFor(1.2f, StartTurn);
        StartCoroutine(cor);
    }

    private IEnumerator ApplyingTurn(LogNote note)
    {
        foreach (Transform child in cardSlot)
            Destroy(child.gameObject);
        var card = Instantiate(player.cardPref, cardSlot).GetComponent<Card>();
        var cardChar = Account.GetGlobalCard(int.Parse(note.CardID));
        card.LoadFrom(cardChar);
        var offset = 2.7f;
        card.GetComponent<RectTransform>().position -= Vector3.one*offset;
        var time = 1f;
        var left = time;
        while (left > 0)
        {
            var dt = Time.deltaTime;
            yield return Waiters.LoopFor(dt, () => card.GetComponent<RectTransform>().position += Vector3.one*dt*offset/time);
            left -= dt;
        }

        
        //yield return new WaitForSeconds(3);
        var selections = Parser.ParseSelections(note.Selections);
        gameEngine.LoadOpponentActions(
            cardChar,
            selections);
        StartCoroutine(Waiters.LoopWhile(
            () => !gameEngine.loaded,
            () => { },
            () =>
            {
                if (note.CompletedTemplate != "")
                    gameEngine.RemoveTemplatesFromBoard(
                        note.CompletedTemplate
                            .FromJsonList()
                            .Select(Parser.GetPositionedTemplateFromString));
            }));
        StartCoroutine(Waiters.LoopFor(3, () =>
        {
            foreach (Transform child in cardSlot)
                Destroy(child.gameObject);
        }));
    }

    public void EndTurn(CardCharacter card)
    {
        var destroyedCard = FindObjectsOfType<Card>().First(c => c.cardCharacter == card);
        Destroy(destroyedCard);
        player.Character.HandList.Remove(card.Id);
        player.Character.GraveList.Add(card.Id);
        player.Character.Push();
        var note = new LogNote(player.Character.Login,
            card,
            gameEngine.SelfSelections,
            gameEngine.LastCompletedTemplates);
        Account.Room.Data.LogList.Add(note);
        Account.Room.Push();
        //print(Account.Room.Data.Log);
        Connector.SendRoom(Account.Room.Name.ToSystemRoom(), Account.Token, Account.Room.DataString);

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