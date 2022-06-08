using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public string FirstPlayerString;
    public string SecondPlayerString;
    public string Log;
    public string Status;
    public PlayerCharacter FirstPlayer;
    public PlayerCharacter SecondPlayer;
    public List<LogNote> LogList;

    public RoomData()
    {
        FirstPlayer = new PlayerCharacter();
        SecondPlayer = new PlayerCharacter();
        LogList = new List<LogNote>();
        Push();
    }

    public void Push()
    {
        FirstPlayerString = JsonUtility.ToJson(FirstPlayer);
        SecondPlayerString = JsonUtility.ToJson(SecondPlayer);
        Log = LogList.ToJsonList();
    }

    public void Pull()
    {
        FirstPlayer = JsonUtility.FromJson<PlayerCharacter>(FirstPlayerString);
        SecondPlayer = JsonUtility.FromJson<PlayerCharacter>(SecondPlayerString);
        LogList = Log.JsonsFromJsonList<LogNote>();
    }
}