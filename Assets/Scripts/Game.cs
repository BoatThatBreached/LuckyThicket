using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Color = UnityEngine.Color;

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
    public Transform cardSlot;
    public bool canDoSomething;
    public GameObject glossary;
    public Popup popup;
    public void EnterDictionary(){
        canDoSomething = false;
        glossary.gameObject.SetActive(true);
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    public void ExitDictionary()
    {
        canDoSomething = true;
        glossary.gameObject.SetActive(false);
        foreach (Transform child in transform)
            child.gameObject.SetActive(true);
    }

    private void Start()
    {
        canDoSomething = true;
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
        foreach (var point in board.Keys.Shuffled())
        {
            AudioStatic.sounds[Basis.Build][Tribes.Beaver]();
            gameEngine.Build(point);
            yield return new WaitForSeconds(Time.deltaTime);
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
        Account.Room = room;
        if (room.Data.Status == $"{opponent.Name} won")
        {
            StartCoroutine(AcceptLoseAndExit());
            return;
        }
        if (room.Data.Status == $"{opponent.Name} lost")
        {
            StartCoroutine(AcceptWinAndExit());
            return;
        }
        Account.Room.Data.FirstPlayer.Pull();
        Account.Room.Data.SecondPlayer.Pull();
        
        player.Init();
        opponent.Init();
        //Account.Room.Push();

        InitCards();
        var lastPlayer = Account.Room.LastTurn ?? Account.Room.Data.SecondPlayer.Login;
        isMyTurn = lastPlayer != Account.Nickname;
        turnText.text = isMyTurn ? "Ваш ход!" : "Ход противника!";
        // TODO: Popup
        if (isMyTurn)
        {
            print(Account.Room.Data.Status);
            if (Account.Room.Data.LogList.Count > 0)
                StartCoroutine(ApplyingTurn(Account.Room.Data.LogList.Last()));
            return;
        }

        print("fetching!");
        StartCoroutine(Waiters.LoopFor(1f, StartTurn));
    }

    private IEnumerator ApplyingTurn(LogNote note)
    {
        foreach (Transform child in cardSlot)
            Destroy(child.gameObject);
        var card = Instantiate(player.cardPref, cardSlot).GetComponent<Card>();
        card.GetComponent<Card>().unplayable = true;
        card.game = this;
        var cardChar = Account.GetGlobalCard(int.Parse(note.CardID));
        card.LoadFrom(cardChar);

        const float offset = 2.7f;
        var rightDown = new Vector3(1, -1, 0);
        card.GetComponent<RectTransform>().position -= rightDown * offset;
        const float time = 1f;
        var left = time;
        while (left > 0)
        {
            var dt = Time.deltaTime;
            yield return Waiters.LoopFor(dt,
                () => card.GetComponent<RectTransform>().position += rightDown * dt * offset / time);
            left -= dt;
        }

        var selections = Parser.ParseSelections(note.Selections);
        var templatesToDestroy = new List<PositionedTemplate>();
        if (note.CompletedTemplate != "")
            templatesToDestroy = note.CompletedTemplate
                .FromJsonList()
                .Select(Parser.GetPositionedTemplateFromString).ToList();
        gameEngine.LoadOpponentActions(
            cardChar,
            selections,
            templatesToDestroy);
        
        StartCoroutine(Waiters.LoopFor(3, () =>
        {
            foreach (Transform child in cardSlot)
                Destroy(child.gameObject);
        }));
    }

    public void EndTurn(LogNote note)
    {
        player.Character.Push();
        opponent.Character.Push();
        
        Account.Room.Data.LogList.Add(note);
        Account.Room.Data.Status = $"Playing continues! Please make turn {opponent.Name}";
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

    public void Win()
    {
        Account.Room.Data.Status = $"{Account.Room.Me.Login} won";
        Account.Room.Data.Push();
        Connector.SendRoom(Account.Room.Name.ToSystemRoom(), Account.Token, Account.Room.DataString);
        StartCoroutine(WinAndExit());
    }

    private IEnumerator WinAndExit()
    {
        yield return popup.ShowMessage("Вы победили! Поздравляем!", Color.yellow);
        SceneManager.LoadScene("RoomScene");
    }
    private IEnumerator LoseAndExit()
    {
        yield return popup.ShowMessage("Не расстраивайтесь! Вы играли достойно!", Color.yellow);
        SceneManager.LoadScene("RoomScene");
    }
    private IEnumerator AcceptWinAndExit()
    {
        Connector.DestroyRoom(Account.Token, Account.Room.Name.ToSystemRoom());
        yield return popup.ShowMessage("Вы победили! Поздравляем!", Color.yellow);
        SceneManager.LoadScene("RoomScene");
    }
    private IEnumerator AcceptLoseAndExit()
    {
        Connector.DestroyRoom(Account.Token, Account.Room.Name.ToSystemRoom());
        yield return popup.ShowMessage("В другой раз обязательно повезёт!", Color.yellow);
        SceneManager.LoadScene("RoomScene");
    }

    public void Lose()
    {
        Account.Room.Data.Status = $"{Account.Room.Me.Login} lost";
        Account.Room.Data.Push();
        Connector.SendRoom(Account.Room.Name.ToSystemRoom(), Account.Token, Account.Room.DataString);
        StartCoroutine(WinAndExit());
    }

    public void SelectCard(Card card)
    {
        var cardChar = new CardCharacter(card.cardCharacter);
        Destroy(card.gameObject);
        player.Character.HandList.Remove(cardChar.Id);
        player.Character.Push();
        gameEngine.LoadSelfActions(card.cardCharacter);
    }
}