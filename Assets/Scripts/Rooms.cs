using System.Collections.Generic;
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
        
        //Connector.DestroyRoom(Account.Token, "Egypt".ToSystemRoom());
        Fetch();
        AudioStatic.MenuInitSounds(this, gameObject);
    }

    private void Fetch()
    {
        var cor = Waiters.LoopFor(1.2f, Fetch);
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
            button.transform.GetChild(1).GetComponent<TMP_Text>().text = room.Data.FirstPlayer.Login;
            button.transform.GetChild(2).GetComponent<TMP_Text>().text = room.Data.SecondPlayer.Login;
            Color color;
            if (room.IsFull)
                color = room.IsHere(Account.Nickname) ? Color.yellow : Color.red;
            else
                color = room.IsHere(Account.Nickname) ? Color.white : Color.green;
            button.GetComponent<Image>().color = color;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioStatic.PlayAudio(AudioStatic.Click);
                Play(room);
            });
        }
    }

    public void Exit() => SceneManager.LoadScene("MenuScene");
    
    private void Play(Room room)
    {
        if (room.IsFull&&!room.IsHere(Account.Nickname)||!room.IsFull&&room.IsHere(Account.Nickname))
            return;
        if (!room.IsFull)
        {
            print(Connector.JoinRoom(Account.Token, room.Name.ToSystemRoom()));
            room.Data.SecondPlayer.Init();
            room.Push();
            print(Connector.SendRoom(room.Name.ToSystemRoom(), Account.Token, room.DataString));
            RefreshRooms();
        }
        else
        {
            room.Data.FirstPlayer.Pull();
            room.Data.SecondPlayer.Pull();
            print(room.Data.Log);
            Account.Room = room;
            SceneManager.LoadScene("GameScene");
        }
    }

    //BEBRA
    public void CreateRoom()
    {
        var room = new Room();
        room.Data.FirstPlayer.Init();
        room.Push();
        print(room.Data.FirstPlayer.Login);
        print(Connector.CreateRoom(Account.Token, roomName.text.ToSystemRoom()));
        print(Connector.SendRoom(roomName.text.ToSystemRoom(), Account.Token, room.DataString));
        RefreshRooms();
    }
}