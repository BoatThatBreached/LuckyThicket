using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Rooms : MonoBehaviour
{
    public GameObject roomPref;
    public Transform roomPanel;
    public TMP_InputField roomName;
    private List<Room> rooms;
    private void Start()
    {
        Fetch();
    }

    private void Fetch()
    {
        var cor = Waiters.LoopFor(10, Fetch);
        RefreshRooms();
        StartCoroutine(cor);
    }

    private void RefreshRooms()
    {
        foreach (Transform child in roomPanel)
            Destroy(child.gameObject);
        rooms = Connector.GetRoomsList();
        foreach (var room in rooms)
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
                print($"BEBROCHKA IN ROOM {room.Name}");
                //Play();
            }
        }
    }

    private void Play()
    {
        print("BEBROCHKA!!!!!");
    }

    //BEBRA
    public void CreateRoom()
    {
        if(rooms.Count(r => r.IsHere(Account.Nickname))<3)
            Connector.CreateRoom(Account.Token, roomName.text);
        RefreshRooms();
    }
}

public class Room
{
    public string Name { get; set; }
    public string FirstPlayer { get; set; }
    public string SecondPlayer { get; set; }

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
}