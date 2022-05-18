using UnityEngine;

public class Room
{
    public string Name;
    public string FPLegacy;
    public string SPLegacy;
    public string LastTurn;
    
    public string DataString;
    public RoomData Data;
    
    public Room()
    {
        Data = new RoomData();
        Push();
    }

    public void Push()
    {
        Data.Push();
        DataString = JsonUtility.ToJson(Data);
    }

    public void Pull()
    {
        Data = JsonUtility.FromJson<RoomData>(DataString);
    }


    public bool IsFull => Data.SecondPlayer.Login != "";
    public bool IsHere(string login) => Data.FirstPlayer.Login == login || Data.SecondPlayer.Login == login;

    public PlayerCharacter Other(string nickname) => nickname == Data.FirstPlayer.Login ? Data.SecondPlayer : Data.FirstPlayer;
    public PlayerCharacter Me(string nickname) => nickname != Data.FirstPlayer.Login ? Data.SecondPlayer : Data.FirstPlayer;

    public static Room CreateFromJson(string json)
    {
        var room = JsonUtility.FromJson<Room>(json);
        room.DataString = json.GetJsons()[0];
        room.Data = JsonUtility.FromJson<RoomData>(room.DataString);
        room.Data.FirstPlayerString = room.DataString.GetJsons()[0].Replace("\\", "");
        room.Data.SecondPlayerString = room.DataString.GetJsons()[1].Replace("\\", "");
       
        room.Data.Pull();
        room.Push();
        return room;
    }
}