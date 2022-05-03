using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Rooms : MonoBehaviour
{
    public GameObject roomPref;
    public Transform roomPanel;
    public TMP_InputField roomName;
    private List<Room> _rooms;
    
    private void Start()
    {
        Fetch();
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
            var s = room.Name;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                Connector.JoinRoom(Account.Token, s);
                RefreshRooms();
            });
            if (room.IsValid(Account.Nickname))
            {
                //print($"BEBROCHKA IN ROOM {room.Name}");
                Account.Room = room;
                Play();
            }
        }
    }

    public void Exit() => SceneManager.LoadScene("MenuScene");
    
    private void Play()
    {
        
        SceneManager.LoadScene("GameScene");
    }

    //BEBRA
    public void CreateRoom()
    {
        print(roomName.text);
        if(_rooms.Count(r => r.IsHere(Account.Nickname))<3)
            Connector.CreateRoom(Account.Token, roomName.text);
        RefreshRooms();
    }
}

public class Room
{
    public string Name;
    public string FirstPlayer;
    public string SecondPlayer;
    public string lastTurn;

    public bool IsValid(string login) =>
        IsHere(login) &&
        FirstPlayer != SecondPlayer
        && FirstPlayer != string.Empty
        && SecondPlayer != string.Empty;

    public bool IsHere(string login) => FirstPlayer == login || SecondPlayer == login;
    
    public Room(string name, string f, string s)
    {
        Name = name;
        FirstPlayer = f;
        SecondPlayer = s;
    }

    public override string ToString()
    {
        return $"{Name}:{FirstPlayer} and {SecondPlayer}";
    }
}