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
    void Start()
    {
        RefreshRooms();
    }

    void RefreshRooms()
    {
        
        foreach(Transform child in roomPanel)
            Destroy(child.gameObject);
        var list = Connector.GetRoomsList();
        var rooms = list
            //.Where(room => room.FirstPlayer != Account.Nickname && room.SecondPlayer != string.Empty)
            ;
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
        }

        foreach (var room in rooms)
        {
            if (room.IsValid)
            {
                print($"BEBROCHKA IN ROOM {room.Name}");
                //Play();
                return;
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
        Connector.CreateRoom(Account.Token, roomName.text);
        RefreshRooms();
    }
}

public class Room
{
    public string Name { get; set; }
    public string FirstPlayer { get; set; }
    public string SecondPlayer { get; set; }
    public bool IsValid => FirstPlayer != SecondPlayer 
                           && FirstPlayer != string.Empty 
                           && SecondPlayer != string.Empty;

    public Room(string name, string f, string s)
    {
        Name = name;
        FirstPlayer = f;
        SecondPlayer = s;
    }
}