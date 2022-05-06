using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class Rooms : MonoBehaviour
{
    public GameObject roomPref;
    public Transform roomPanel;
    public TMP_InputField roomName;
    private List<Room> _rooms;

    private void Start()
    {

        //Connector.DestroyRoom(Account.Token, "GW".ToSystemRoom());
        Fetch();
        
        AudioStatic.AddMainTheme(AudioStatic.MainTheme, gameObject);
        AudioStatic.AddSoundsToButtons(AudioStatic.Click, gameObject);
    }

    private void Fetch()
    {
        var cor = Waiters.LoopFor(2, Fetch);
        RefreshRooms();
        StartCoroutine(cor);
    }

    private void RefreshRooms()
    {
        foreach (Transform child in roomPanel)
            Destroy(child.gameObject);
        _rooms = Connector.GetRoomsList();
        foreach (var room in _rooms)
        {
            var button = Instantiate(roomPref, roomPanel);
            button.transform.GetChild(0).GetComponent<TMP_Text>().text = room.Name;
            button.transform.GetChild(1).GetComponent<TMP_Text>().text = room.FirstPlayer;
            button.transform.GetChild(2).GetComponent<TMP_Text>().text = room.SecondPlayer;
            var color = new Color(1, 1, 1);
            if (room.IsFull)
                color = room.IsHere(Account.Nickname) ? Color.yellow : Color.red;
            else
                color = room.IsHere(Account.Nickname) ? Color.white : Color.green;
            button.GetComponent<Image>().color = color;
            var s = room.Name;
            button.GetComponent<Button>().onClick.AddListener(() => Play(room));
        }
    }

    public void Exit() => SceneManager.LoadScene("MenuScene");
    
    private void Play(Room room)
    {
        if (room.IsFull&&!room.IsHere(Account.Nickname))
            return;
        if (!room.IsFull)
        {
            print(Connector.JoinRoom(Account.Token, room.Name.ToSystemRoom()));
            RefreshRooms();
        }
        else
        {
            //if(room.FirstPlayer==Account.Nickname)
            //    print(Connector.SendRoom(room.Name.ToSystemRoom(), Account.Token, Parser.ConvertBoardToJson(room.Board)));
            Account.Room = room;
            SceneManager.LoadScene("GameScene");
        }
    }

    //BEBRA
    public void CreateRoom()
    {
        print(Connector.CreateRoom(Account.Token, roomName.text.ToSystemRoom()));
        RefreshRooms();
    }
}

public class Room
{
    public string Name;
    public string FirstPlayer;
    public string SecondPlayer;
    public string LastTurn;
    public Dictionary<Point, Tribes> Board;

    public bool IsFull => SecondPlayer != "";

    public bool IsHere(string login) => FirstPlayer == login || SecondPlayer == login;

    public string BoardString()
    {
        var res = "";
        var minX = Board.Keys.Select(p => p.X).Min();
        var minY = Board.Keys.Select(p => p.Y).Min();
        var maxX = Board.Keys.Select(p => p.X).Max();
        var maxY = Board.Keys.Select(p => p.Y).Max();
        for (var j = maxY; j >= minY; j--)
        {
            var line = "";
            for (var i = minX; i <= maxX; i++)
            {
                var p = new Point(i, j);
                var ch = !Board.ContainsKey(p) ? "x" : Board[p].ToString()[0].ToString().ToLower();
                line += ch;
            }

            res += line + '\n';
        }

        return res;
    }

    public Room(string name, string f, string s)
    {
        Name = name;
        FirstPlayer = f;
        SecondPlayer = s;
    }

    public override string ToString()
    {
        return $"{Name}:{FirstPlayer} and {SecondPlayer}. LastTurn was made by {LastTurn}, board is {BoardString()}";
    }

    public string ToJson()
    {
        var res = "{";
        res += $"\"Name\":\"{Name}\",";
        res += $"\"FirstPlayer\":\"{FirstPlayer}\",";
        res += $"\"SecondPlayer\":\"{SecondPlayer}\",";
        res += $"\"LastTurn\":\"{LastTurn}\",";
        res += $"\"Board\":{Parser.ConvertBoardToJson(Board)}";
        res += "}";
        return res;
    }

    public string Other(string nickname)
    {
        //assuming nickname already joined this room
        return nickname == FirstPlayer ? SecondPlayer : FirstPlayer;
    }
}