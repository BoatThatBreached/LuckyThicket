using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        var list = Connector.GetRoomsList();
    }

    void CreateRoom()
    {
        Connector.CreateRoom(Account.Token, roomName.text);
        RefreshRooms();
    }
}
